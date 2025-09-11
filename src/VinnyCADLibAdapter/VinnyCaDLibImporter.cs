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
using CADLib;
using CADLibKernel;

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

        private void ProcessObject(StructureInfo obj, CADLibKernel.CSElement parentElement, List<int> parentMiniCatalogues)
        {
            VinnyLibDataStructureObject VinnyObject = mVinnyData.ObjectsManager.GetObjectById(obj.Id);
            //Создадим и заполним временный CSElement

            CADLibKernel.CSElement csElement;
            int parentCSid;
            if (parentElement == null) 
            {
                CADLibKernel.CSElement rootElement = new CADLibKernel.CSElement(Path.GetFileNameWithoutExtension(mInputParams.Path));
                rootElement.AddParameter(CADLibData.CADLibParameter_BUILDINGS_STRUCT_LEVEL, "1. Площадка (Генплан)", CADLibData.CADLibParameterLABEL_BUILDINGS_STRUCT_LEVEL, "");
                parentCSid = DBPublisher.PublishElement(rootElement, CADLibData.CADLibCategory_StructureData, 1);
                parentElement = rootElement;
            }
            parentCSid = parentElement.Id;
            csElement = new CADLibKernel.CSElement(VinnyObject.Name);

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
            // Добавляем объект в соответствующий миникаталог и во все родительские миникаталоги (отложенное назначение)
            var parentMiniCatalogues2 = parentMiniCatalogues.Concat(new int[] { vinnyObject2CADLibMCatalogId[obj.Id] }).ToList();

            foreach (int parentMCid in parentMiniCatalogues2)
            {
                CADLibMCatalogs2Objects[parentMCid].Add(idObject);
            }

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
                ProcessObject(subObject, csElement, parentMiniCatalogues2);
            }
        }

        //Создание миникаталогов
        private void ProcessObject2(StructureInfo obj, CLibCatalogFilterItem parentMC)
        {
            VinnyLibDataStructureObject VinnyObject = mVinnyData.ObjectsManager.GetObjectById(obj.Id);
            CLibCatalogFilterItem mc = CADLibData.CADLibrary3D.CreateDirectory(parentMC, Guid.NewGuid().ToString("N"), VinnyObject.Name, null);
            SetAcess(mc);
            vinnyObject2CADLibMCatalogId.Add(obj.Id, mc.nDirectory);
            CADLibMCatalogs2Objects.Add(mc.nDirectory, new List<int>());

            foreach (var subObject in obj.Childs)
            {
                ProcessObject2(subObject, mc);
            }
        }

        private List<int> CADLibGroups;
        private void SetAcess(CLibCatalogFilterItem item)
        {
            try
            {
                CADLibData.CADLibrary3D.SetFolderAccess(item.nCatID, CADLibGroups);
            }
            catch { }
        }

        private void AddObjectsToDirectory(int nObjectId, int nDir)
        {
            CADLibData.CADLibrary3D.AddToDirectory(nObjectId, nDir);
        }

        /// <summary>
        /// Сопоставление идентификаторов объектов в VinnyData.ObjectsManager.Objects идентификаторам миникаталогов в CADLib
        /// </summary>
        private Dictionary<int, int> vinnyObject2CADLibMCatalogId;

        /// <summary>
        /// Объекты в миникаталогах. Исполнить после подтверждения публикации, иначе будет ошибка
        /// </summary>
        private Dictionary<int, List<int>> CADLibMCatalogs2Objects;

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

            // Создание специальной категории объектов для импортируемых данных по умолчанию
            if (CADLibData.CADLibrary3D.GetCategoryInfoByName(mVinnyCADLibCategory) == null)
            {
                mVinnyObjectsCategory = CADLibData.CADLibrary3D.CreateCategory(mVinnyCADLibCategory, "Объекты созданные через VinnyConverter");
            }
            else mVinnyObjectsCategory = CADLibData.CADLibrary3D.GetCategoryInfoByName(mVinnyCADLibCategory).idCategory;

            // Подготовительные действия для создания миникаталогов
            vinnyObject2CADLibMCatalogId = new Dictionary<int, int>();
            CADLibMCatalogs2Objects = new Dictionary<int, List<int>>();
            int group_need = CADLibData.CADLibrary3D.GetUserGroupBySysName("ALL");
            CADLibGroups = new List<int>() { group_need };

            // Создание корневого миникаталога для результатов импорта данных в CADLib
            CLibCatalogFilterItem vinnyMCatalogRoot = CADLibData.CADLibrary3D.GetFolderByPath("F:Результаты импорта данных через VinnyConverter");
            if (vinnyMCatalogRoot == null) 
            {
                vinnyMCatalogRoot = CADLibData.CADLibrary3D.CreateDirectory(null, "VinnyImport", "Результаты импорта данных через VinnyConverter", null);
                SetAcess(vinnyMCatalogRoot);
            }

            // Создание миникаталога для данного сеанса импорта
            CLibCatalogFilterItem vinnyMCatalogCurrent =
                CADLibData.CADLibrary3D.CreateDirectory(vinnyMCatalogRoot, $"{Guid.NewGuid().ToString("N")}",
                $"{Path.GetFileNameWithoutExtension(openParameters.Path)} {DateTime.Now.ToString("G")}", null);
            SetAcess(vinnyMCatalogCurrent);
            CADLibMCatalogs2Objects.Add(vinnyMCatalogCurrent.nDirectory, new List<int>());

            // Получение структуры данных
            VinnyLibDataStructureObjectsManager.StructureInfo[] vinnyModelStructureInfo = mVinnyData.ObjectsManager.GetAllStructure();

            // Первый прогон ради создания миникаталогов
            foreach (VinnyLibDataStructureObjectsManager.StructureInfo vinnyModelStructureGroupInfo in vinnyModelStructureInfo)
            {
                ProcessObject2(vinnyModelStructureGroupInfo, vinnyMCatalogCurrent);
            }

            // Второй прогон ради импорта объектов и вставки их в созданные миникаталоги используя vinnyObject2CADLibMCatalogId
            foreach (VinnyLibDataStructureObjectsManager.StructureInfo vinnyModelStructureGroupInfo in vinnyModelStructureInfo)
            {
                ProcessObject(vinnyModelStructureGroupInfo, null, new List<int>() { vinnyMCatalogCurrent .nDirectory});
            }


            //Завершение публикации и запуск вспомогательной утилиты по записи содержимого дамп-файлов в БД
            DBPublisher.CommitPublication();
            DBPublisher.RunCommitUtility();

            // Исполняем CADLibMCatalogs2Objects
            foreach (var mcInfo in CADLibMCatalogs2Objects)
            {
                foreach (int id in mcInfo.Value) CADLibData.CADLibrary3D.AddToDirectory(id, mcInfo.Key);
            }

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
