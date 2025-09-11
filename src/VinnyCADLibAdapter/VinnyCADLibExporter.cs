using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinnyCADLibAdapter.CADLibProcessing;
using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;
using VinnyLibConverterKernel;

using CADLibKernel;
using ModelStudio.Graphics3D;
using CADLib;

namespace VinnyCADLibAdapter
{
    internal class VinnyCADLibExporter : VinnyLibConverterCommon.Interfaces.ICadExportProcessing
    {
        public static VinnyCADLibExporter CreateInstance()
        {
            if (mInstance == null) mInstance = new VinnyCADLibExporter();
            if (mConverter == null) mConverter = VinnyLibConverter.CreateInstance2();
            return mInstance;
        }

        private const string CADLibParamCategory_Properties = "CADLib Properties";

        /// <summary>
        /// Проверка, попадает ли хоть один объект на сцене в объекты, связанные с данным, либо с его вложенными дочерними
        /// </summary>
        /// <param name="RootObjectId"></param>
        /// <returns></returns>
        private bool IsObjectNeed(int RootObjectId)
        {
            if (!mCurrentCADLibObjectsData.Object2ChildsInfoExtended.ContainsKey(RootObjectId)) return false;
            var ids = mCurrentCADLibObjectsData.Object2ChildsInfoExtended[RootObjectId];
            foreach (var id in ids)
            {
                if (this.mActiveObjectIds.Contains(id)) return true;
            }
            return false;
        }

        /// <summary>
        /// Сопоставление мэшей CADLib и созданных на его базе наших мэшей
        /// </summary>
        private Dictionary<int, int> cadlibMesh2VinnyMeshes = new Dictionary<int, int>();
        private void ProcessCadlibObject(StructureInfo CLobject, int parentObjectId)
        {
            //if (!IsObjectNeed(CLobject.IdObject)) return;

            VinnyLibDataStructureObject vinnyObjectDef = mVinnyModelDef.ObjectsManager.GetObjectById(mVinnyModelDef.ObjectsManager.CreateObject());
            CLibObjectInfo CADLibObjectDef = this.mCurrentCADLibObjectsData.Objects[CLobject.IdObject];
            vinnyObjectDef.Name = CADLibObjectDef.Name;
            vinnyObjectDef.UniqueId = CADLibObjectDef.UID.ToString("N");
            vinnyObjectDef.ParentId = parentObjectId;

            vinnyObjectDef.Parameters.Add(mVinnyModelDef.ParametersManager.CreateParameterValueWithDefs("CADLibObjectGuid", CADLibObjectDef.UID.ToString("N"), CADLibParamCategory_Properties));
            vinnyObjectDef.Parameters.Add(mVinnyModelDef.ParametersManager.CreateParameterValueWithDefs("CADLibObjectId", CADLibObjectDef.idObject, CADLibParamCategory_Properties));

            //properties
            if (mCurrentCADLibObjectsData.ObjectsProperties.ContainsKey(CLobject.IdObject))
            {
                foreach (var CADLibParamInfo in mCurrentCADLibObjectsData.ObjectsProperties[CLobject.IdObject])
                {
                    vinnyObjectDef.Parameters.Add(mVinnyModelDef.ParametersManager.CreateParameterValueWithDefs(CADLibParamInfo.ParamDef.Name, CADLibParamInfo.Value, CADLibParamCategory_Properties, VinnyLibDataStructureParameterDefinitionType.ParamString, CADLibParamInfo.ParamDef.Caption));
                }
            }

            //geometry
            var graphicsInfo = CADLibData.CADLibrary3D.GetObject3DShapesInfo(CLobject.IdObject);
            if (graphicsInfo != null && graphicsInfo.Any())
            {
                foreach (CSShapeInfo objectGraphicsShape in graphicsInfo)
                {
                    //Создаем определение геометрии
                    if (!cadlibMesh2VinnyMeshes.ContainsKey(objectGraphicsShape.nMeshId))
                    {
                        VinnyLibDataStructureGeometryMesh vinnyGeometry = mVinnyModelDef.GeometrtyManager.GetMeshGeometryById(mVinnyModelDef.GeometrtyManager.CreateGeometry(VinnyLibDataStructureGeometryType.Mesh));

                        CSVectorD3 ptBase;
                        var arrMeshBytes = CADLibData.CADLibrary3D.Download3DMesh(objectGraphicsShape.nMeshId, 0, out ptBase);
                        CSMesh meshDef = new CSMesh();
                        meshDef.LoadFromBuffer(arrMeshBytes);
                        //meshDef.TransformBy(objectGraphicsShape.GetCoordSystem());

                        for (int vx_counter = 0; vx_counter < meshDef.Vertices.Count; vx_counter++)
                        {
                            var vertexDef = meshDef.Vertices[vx_counter].Position;
                            vinnyGeometry.AddVertex(
                                vertexDef.x + (float)ptBase.x,
                                vertexDef.y + (float)ptBase.y,
                                vertexDef.z + (float)ptBase.z);
                        }

                        int tr_counter = 0;
                        for (int f_counter = 0; f_counter < meshDef.Index.Count; f_counter += 3)
                        {
                            int vx_1 = meshDef.Index[f_counter];
                            int vx_2 = meshDef.Index[f_counter + 1];
                            int vx_3 = meshDef.Index[f_counter + 2];

                            //if (new int[] { vx_1, vx_2, vx_3 }.Distinct().Count() == 3)
                            vinnyGeometry.AddFace(vx_1, vx_2, vx_3);

                            tr_counter++;
                        }

                        var material_first = meshDef.Materials[0];
                        vinnyGeometry.MaterialId = mVinnyModelDef.MaterialsManager.CreateMaterial(new int[] { Convert.ToInt32(material_first.vAmbient.x), Convert.ToInt32(material_first.vAmbient.y), Convert.ToInt32(material_first.vAmbient.z), Convert.ToInt32(material_first.fAlpha) });

                        mVinnyModelDef.GeometrtyManager.SetMeshGeometry(vinnyGeometry.Id, vinnyGeometry);
                        cadlibMesh2VinnyMeshes.Add(objectGraphicsShape.nMeshId, vinnyGeometry.Id);
                    }
                    //Создаеи определение положения геометрии
                    VinnyLibDataStructureGeometryPlacementInfo vinnyGeomPI = mVinnyModelDef.GeometrtyManager.GetGeometryPlacementInfoById(mVinnyModelDef.GeometrtyManager.CreateGeometryPlacementInfo(cadlibMesh2VinnyMeshes[objectGraphicsShape.nMeshId]));
                    /*
                    vinnyGeomPI.Position = new float[] {
                        (float)objectGraphicsShape.ptPosition.x,
                        (float)objectGraphicsShape.ptPosition.y,
                        (float)objectGraphicsShape.ptPosition.z,
                    };
                    */
                    var matrix = objectGraphicsShape.GetCoordSystem().Matr;
                    float[] matrix2 = matrix.Select(a => Convert.ToSingle(a)).ToArray();
                    vinnyGeomPI.TransformatiomMatrix = matrix2;
                    mVinnyModelDef.GeometrtyManager.SetMeshGeometryPlacementInfo(vinnyGeomPI.Id, vinnyGeomPI);

                    vinnyObjectDef.GeometryPlacementInfoIds.Add(vinnyGeomPI.Id);
                }
            }
            
            foreach (StructureInfo CLchildObject in CLobject.Childs)
            {
                ProcessCadlibObject(CLchildObject, vinnyObjectDef.Id);
            }
            mVinnyModelDef.ObjectsManager.SetObject(vinnyObjectDef.Id, vinnyObjectDef);
        }
        public VinnyLibDataStructureModel CreateData()
        {
            mVinnyModelDef = new VinnyLibDataStructureModel();
            cadlibMesh2VinnyMeshes = new Dictionary<int, int>();

            //1. Необходимо получить полное дерево иерархии по структуре
            mCurrentCADLibObjectsData = new ModelInfo();
            mCurrentCADLibObjectsData.InitializeData();
            //2. Необходимо получить набор объектов, отрисованных на виде
            mActiveObjectIds = CADLibData.CADLibrary3D.GetCurrentViewObjects().ToArray();
            //TODO: если вернёт не то, то перенести из Collide чтение таблицы UserCurrentObjects
            //Для найденных объектов необходимо установить полную связь со структурой модели mCurrentCADLibObjectsData
            //Для этого имеется свойство Object2ChildsInfoExtended и ModelStructure. Если член иерархии содержит объект, то по нему идём

            VinnyLibDataStructureObject vinnyNavisScene = mVinnyModelDef.ObjectsManager.GetObjectById(mVinnyModelDef.ObjectsManager.CreateObject());
            vinnyNavisScene.Name = "CADLib scene";

            //header
            foreach (var CADLibParamInfo in mCurrentCADLibObjectsData.ObjectsProperties[mCurrentCADLibObjectsData.ProjectObjectId])
            {
                vinnyNavisScene.Parameters.Add(mVinnyModelDef.ParametersManager.CreateParameterValueWithDefs(CADLibParamInfo.ParamDef.Name, CADLibParamInfo.Value, CADLibParamCategory_Properties, VinnyLibDataStructureParameterDefinitionType.ParamString, CADLibParamInfo.ParamDef.Caption));
            }

            //Всё равно получить иерархию не получается, пусть хоть экспортируется только видимое, а не всё сразу.
            mCurrentCADLibObjectsData.ModelStructure.Childs = new List<StructureInfo>();
            foreach (int activeObjectId in mActiveObjectIds)
            {
                mCurrentCADLibObjectsData.ModelStructure.Childs.Add(new StructureInfo() { IdObject = activeObjectId });

            }

            foreach (StructureInfo CLstructureItem in mCurrentCADLibObjectsData.ModelStructure.Childs)
            {
                ProcessCadlibObject(CLstructureItem, vinnyNavisScene.Id);
            }
            mVinnyModelDef.ObjectsManager.SetObject(vinnyNavisScene.Id, vinnyNavisScene);

            return mVinnyModelDef;
        }
        public void ExportTo(VinnyLibDataStructureModel vinnyData, ImportExportParameters outputParameters)
        {
            mConverter.ExportModel(vinnyData, outputParameters);
        }

        private int[] mActiveObjectIds = new int[0];
        private VinnyLibDataStructureModel mVinnyModelDef;
        private ModelInfo mCurrentCADLibObjectsData;
        private static VinnyCADLibExporter mInstance;
        private static VinnyLibConverterKernel.VinnyLibConverter mConverter;
    }
}
