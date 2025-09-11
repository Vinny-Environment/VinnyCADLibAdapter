using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VinnyLibConverterCommon.VinnyLibDataStructure;
using VinnyLibConverterCommon;
using static VinnyLibConverterCommon.VinnyLibDataStructure.VinnyLibDataStructureObjectsManager;
using VinnyLibConverterKernel;

using ModelStudio.Graphics3D;
using System.IO;

namespace VinnyCADLibAdapter
{
    internal class VinnyCaDLibImporter : VinnyLibConverterCommon.Interfaces.ICadImportProcessing
    {
        public static VinnyCaDLibImporter CreateInstance()
        {
            if (mInstance == null) mInstance = new VinnyCaDLibImporter();
            if (mConverter == null) mConverter = VinnyLibConverter.CreateInstance2();
            return mInstance;
        }

        private void ProcessObject(StructureInfo obj, CADLibKernel.CSElement parentElement)
        {
            VinnyLibDataStructureObject VinnyObject = mVinnyData.ObjectsManager.GetObjectById(obj.Id);
            //Создадим и заполним временный CSElement

            CADLibKernel.CSElement csElement;
            if (parentElement == null) 
            {
                CADLibKernel.CSElement rootElement = new CADLibKernel.CSElement(Path.GetFileNameWithoutExtension(mInputParams.Path));
                rootElement.AddParameter(CADLibData.CADLibParameter_BUILDINGS_STRUCT_LEVEL, "1. Площадка (Генплан)", CADLibData.CADLibParameterLABEL_BUILDINGS_STRUCT_LEVEL, "");
                DBPublisher.PublishElement(rootElement, CADLibData.CADLibCategory_StructureData, 1);
                parentElement = rootElement;
            }
            csElement = new CADLibKernel.CSElement(VinnyObject.Name, parentElement);

            string vinnyCADLib_StructureParamRaw = "";
            string vinnyCADLib_Category = mVinnyCADLibCategory;
            foreach (var param in VinnyObject.Parameters)
            {
                var paramDef = mVinnyData.ParametersManager.GetParamDefById(param.ParamDefId);
                csElement.AddParameter(paramDef.Name, param.ToString(), paramDef.Caption, "");

                if (paramDef.Name == mCADLibParam_VinnyCADLibObjectType) vinnyCADLib_Category = param.ToString();
                if (paramDef.Name == mCADLibParam_VinnyCADLibStructure) vinnyCADLib_StructureParamRaw = param.ToString();
            }

            string vinnyCADLib_StructureParam = "";
            if (vinnyCADLib_StructureParamRaw != "" && CADLibData.CADLibStructureInfo.TryGetValue(vinnyCADLib_StructureParamRaw, out vinnyCADLib_StructureParam))
            {
                csElement.AddParameter(CADLibData.CADLibParameter_BUILDINGS_STRUCT_LEVEL, vinnyCADLib_StructureParam, CADLibData.CADLibParameterLABEL_BUILDINGS_STRUCT_LEVEL, "");
            }

            //Исключение на категорию, которой нет
            if (vinnyCADLib_Category != mVinnyCADLibCategory)
            {
                if (CADLibData.CADLibrary3D.GetCategoryInfoByName(vinnyCADLib_Category) == null) vinnyCADLib_Category = mVinnyCADLibCategory;
            }

            //Запись элемента во временные дамп-файлы
            int idObject = DBPublisher.PublishElement(csElement, vinnyCADLib_Category, 1);

            //Сохранение геометрии

            List<CSMesh> meshList = new List<CSMesh>();
            List<CSMatrixD> matrList = new List<CSMatrixD>();
            List<CSVectorD3> baseList = new List<CSVectorD3>();

            foreach (int VinnyGeomPIid in VinnyObject.GeometryPlacementInfoIds)
            {
                VinnyLibDataStructureGeometryPlacementInfo VinnyGeomPI = mVinnyData.GeometrtyManager.GetGeometryPlacementInfoById(VinnyGeomPIid);
                VinnyLibDataStructureGeometryMesh VinnyMesh = mVinnyData.GeometrtyManager.GetMeshGeometryById(VinnyGeomPI.IdGeometry);

                CSMesh csMesh = new CSMesh();
                //foreach (var point in VinnyMesh.mPoints)
                //{
                //    float[] pointCoords = point.Value;
                //    csMesh.AddVertex(new CSVertexCompressed(new CSVector3(pointCoords[0], pointCoords[1], pointCoords[2]), CSVector3.AxisZ));
                //}

                //Создадим и установим материал
                VinnyLibDataStructureMaterial vinnyMaterialDef = mVinnyData.MaterialsManager.GetMaterialById(VinnyMesh.MaterialId);
                CSMaterial csMaterial = new CSMaterial(
                    Convert.ToByte(vinnyMaterialDef.ColorR),
                    Convert.ToByte(vinnyMaterialDef.ColorG),
                    Convert.ToByte(vinnyMaterialDef.ColorB));
                csMesh.SetCurrentMaterial(csMaterial);

                foreach (var triangle in VinnyMesh.Faces)
                {
                    int[] triangleIndices = triangle.Value;
                    float[] p1 = VinnyMesh.Points[triangleIndices[0]];
                    float[] p2 = VinnyMesh.Points[triangleIndices[1]];
                    float[] p3 = VinnyMesh.Points[triangleIndices[2]];

                    csMesh.Index.Add(csMesh.AddVertex(new CSVertexCompressed(new CSVector3(p1[0], p1[1], p1[2]), CSVector3.AxisZ)));
                    csMesh.Index.Add(csMesh.AddVertex(new CSVertexCompressed(new CSVector3(p2[0], p2[1], p2[2]), CSVector3.AxisZ)));
                    csMesh.Index.Add(csMesh.AddVertex(new CSVertexCompressed(new CSVector3(p3[0], p3[1], p3[2]), CSVector3.AxisZ)));
                    csMesh.MaterialIndex.Add(0);
                }

                meshList.Add(csMesh);

                CSMatrixD transform = new CSMatrixD();
                transform.Set(VinnyGeomPI.TransformationMatrixInfo.Matrix.GetMatrixDouble());
                matrList.Add(transform);
                baseList.Add(new CSVectorD3(0, 0, 0));
            }
            //Публикация геометрии
            DBPublisher.PublishGraphics(meshList, baseList, matrList, idObject, -1, null, null);

            foreach (var subObject in obj.Childs)
            {
                ProcessObject(subObject, csElement);
            }
        }

        public void ImportFrom(ImportExportParameters openParameters)
        {
            mVinnyData = mConverter.ImportModel(openParameters);
            mVinnyData.SetCoordinatesTransformation(openParameters.TransformationInfo);
            mInputParams = openParameters;

            string dumpPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            dumpPath += "\\AppData\\Local\\Temp\\BinaryDat";
            System.IO.Directory.CreateDirectory(dumpPath);
            DBPublisher = new MsBinaryPublishing.MsBinaryPublisher();
            DBPublisher.Connect(CADLibData.CADLibrary3D, null, dumpPath);
            DBPublisher.InitPublication(dumpPath);
            DBPublisher.BeginPublication(mVinnyData.ObjectsManager.mObjects.Count);

            if (CADLibData.CADLibrary3D.GetCategoryInfoByName(mVinnyCADLibCategory) == null)
            {
                mVinnyObjectsCategory = CADLibData.CADLibrary3D.CreateCategory(mVinnyCADLibCategory, "Объекты созданные через VinnyConverter");
            }
            else mVinnyObjectsCategory = CADLibData.CADLibrary3D.GetCategoryInfoByName(mVinnyCADLibCategory).idCategory;

            VinnyLibDataStructureObjectsManager.StructureInfo[] vinnyModelStructureInfo = mVinnyData.ObjectsManager.GetAllStructure();
            foreach (VinnyLibDataStructureObjectsManager.StructureInfo vinnyModelStructureGroupInfo in vinnyModelStructureInfo)
            {
                ProcessObject(vinnyModelStructureGroupInfo, null);
            }


            //Завершение публикации и запуск вспомогательной утилиты по записи содержимого дамп-файлов в БД
            DBPublisher.CommitPublication();
            DBPublisher.RunCommitUtility();
        }

        private const string mCADLibParam_VinnyCADLibObjectType = "VinnyCADLibObjectType";
        private const string mCADLibParam_VinnyCADLibStructure = "VinnyCADLibStructure";

        private ImportExportParameters mInputParams;
        private string mVinnyCADLibCategory = "VinnyObject";
        private int mVinnyObjectsCategory;
        private VinnyLibDataStructureModel mVinnyData;
        private MsBinaryPublishing.MsBinaryPublisher DBPublisher = null;
        private static VinnyLibConverterKernel.VinnyLibConverter mConverter;
        private static VinnyCaDLibImporter mInstance;
    }
}
