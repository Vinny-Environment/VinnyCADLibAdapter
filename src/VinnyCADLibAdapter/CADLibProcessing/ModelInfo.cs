using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CADLibKernel;

namespace VinnyCADLibAdapter.CADLibProcessing
{
    internal class StructureInfo
    {
        public StructureInfo()
        {
            Childs = new List<StructureInfo>();
            IdObject = -1;
        }
        public int IdObject { get; set; }
        public List<StructureInfo> Childs { get; set; }
    }
    internal class ModelInfo
    {
        public Dictionary<int, CLibObjectInfo> Objects { get; private set; }
        public Dictionary<int, List<CADLibraryBase.ParamValue>> ObjectsProperties{ get; private set; }
        public Dictionary<int, string> Categories { get; private set; }
        public Dictionary<Guid, int> ObjectGuid2IdObject { get; private set; }

        public Dictionary<int, List<int>> Object2ChildsInfo { get; private set; }

        /// <summary>
        /// Здесь в значении перечисляются идентификаторы ВСЕХ дочерних (включая вложенные)
        /// </summary>
        public Dictionary<int, List<int>> Object2ChildsInfoExtended { get; private set; }

        public StructureInfo ModelStructure { get; private set; }

        public int ProjectObjectId { get; private set; }
        /// <summary>
        /// Категория объектов "structure_data"
        /// </summary>
        public int CategoryStructureId { get; private set; }

        public int ParamDefId_BUILDINGS_STRUCT_LEVEL { get; private set; }

        public ModelInfo()
        {
            Objects = new Dictionary<int, CLibObjectInfo>();
            ObjectsProperties = new Dictionary<int, List<CADLibraryBase.ParamValue>>();
            Categories = new Dictionary<int, string>();
            ObjectGuid2IdObject = new Dictionary<Guid, int>();
            Object2ChildsInfo = new Dictionary<int, List<int>>();
            Object2ChildsInfoExtended = new Dictionary<int, List<int>>();
            ModelStructure = new StructureInfo();
        }
        public void InitializeData()
        {
            ProjectObjectId = CADLibData.CADLibrary3D.GetProjectObject().idObject;

            ParamDefId_BUILDINGS_STRUCT_LEVEL = CADLibData.CADLibrary3D.GetParamDefId("BUILDINGS_STRUCT_LEVEL");

            //Борьба c теми, кто не входит в m_library.GetCategoriesList()
            for (int cat_counter = 0; cat_counter < 1000; cat_counter++)
            {
                CLibCategoryInfo CADLibCategoryDef = null;
                try
                {
                    CADLibCategoryDef = CADLibData.CADLibrary3D.GetObjectCategory(cat_counter);
                }
                catch (Exception ex) { Debug.Print(ex.Message); }

                if (CADLibCategoryDef != null)
                {
                    Categories.Add(CADLibCategoryDef.idCategory, CADLibCategoryDef.mCaption);
                    if (CADLibCategoryDef.mSysName == "structure_data") CategoryStructureId = CADLibCategoryDef.idCategory;
                    var objects = CADLibData.CADLibrary3D.GetLibraryObjectsByCategory(CADLibCategoryDef.mSysName);
                    if (objects != null && objects.Any())
                    {
                        foreach (CLibObjectInfo CLobject in objects)
                        {
                            Objects.Add(CLobject.idObject, CLobject);
                            ObjectGuid2IdObject.Add(CLobject.UID, CLobject.idObject);
                        }
                    }

                }
            }

            ObjectsProperties = CADLibData.CADLibrary3D.GetObjectsParameters(Objects.Keys);

            foreach (var o in Objects)
            {
                Object2ChildsInfo.Add(o.Value.idObject, new List<int>());

                foreach (var o2 in Objects)
                {
                    if (o2.Value.idParentObject == o.Value.idObject)
                    {
                        Object2ChildsInfo[o.Value.idObject].Add(o2.Value.idObject);
                    }
                }
            }

            //Вычисляем структуру (ищем Площадки -- "1. Площадка (Генплан)")
            List<int> ProjectData = Object2ChildsInfo[ProjectObjectId];

            foreach (var o in Objects)
            {
                if (ObjectsProperties.ContainsKey(o.Value.idObject))
                {
                    var props = ObjectsProperties[o.Value.idObject];
                    var hierProp = props.Where(p => p.ParamDef.Id == ParamDefId_BUILDINGS_STRUCT_LEVEL);
                    if (hierProp.Any() && hierProp.First().Value.ToString() == "1. Площадка (Генплан)")
                    {
                        if (!ProjectData.Contains(o.Value.idObject)) Object2ChildsInfo[ProjectObjectId].Add(o.Value.idObject);
                    } 
                }
            }

            //Теперь у нас есть корректная структура начиная от Проекта из расчета что остальные ветви структуры имеют idParent
            //ProjectData = Object2ChildsInfo[ProjectObjectId]; //для проверки (отладки)

            //Вычисляем Object2ChildsInfoExtended
            foreach (var data in Object2ChildsInfo)
            {
                Object2ChildsInfoExtended.Add(data.Key, new List<int>());
            }

            ModelStructure.IdObject = ProjectObjectId;
            foreach (int subProjectElementId in Object2ChildsInfo[ProjectObjectId])
            {
                ModelStructure.Childs.Add(ProcessingObjectsHierarchy(subProjectElementId, new List<int>() { ProjectObjectId }));
            }

            //убираем дубли
            int[] Object2ChildsInfoExtended_Keys = Object2ChildsInfoExtended.Keys.Cast<int>().ToArray();
            foreach (int dataKey in Object2ChildsInfoExtended_Keys)
            {
                Object2ChildsInfoExtended[dataKey] = Object2ChildsInfoExtended[dataKey].Distinct().ToList();
            }
        }

        private StructureInfo ProcessingObjectsHierarchy(int idObject, List<int> parentObjectsId)
        {
            StructureInfo sInfo = new StructureInfo();
            sInfo.IdObject = idObject;

            //var chInfo = Object2ChildsInfo[idObject];
            foreach (int parentId in parentObjectsId)
            {
                Object2ChildsInfoExtended[parentId].Add(idObject);
            }

            List<int> parentObjectsIdEdited = parentObjectsId.Concat(new int[] { idObject }).ToList();
            CADLibData.CADLibrary3D.GetChildObjects
            foreach (int chObjectId in Object2ChildsInfo[idObject])
            {
                sInfo.Childs.Add(ProcessingObjectsHierarchy(chObjectId, parentObjectsIdEdited));
            }

            foreach (int parentId in parentObjectsId)
            {
                Object2ChildsInfoExtended[parentId] = Object2ChildsInfoExtended[parentId].Concat(Object2ChildsInfoExtended[idObject]).ToList();
            }

            return sInfo;
        }
    }
}
