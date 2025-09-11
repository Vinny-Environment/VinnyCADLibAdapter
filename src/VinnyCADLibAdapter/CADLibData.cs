using CADLib;
using CSProject3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyCADLibAdapter
{
    internal class CADLibData
    {
        public static PluginsManager mManager;

        public static CAD3DLibrary CADLibrary3D => (CAD3DLibrary)mManager.Library;

        public static Dictionary<string, string> CADLibStructureInfo = new System.Collections.Generic.Dictionary<string, string>()
        {
            { "SITE", "1. Площадка (Генплан)" },
            { "BUILDING", "2. Здание (Сооружение)" },
            { "LEVEL", "3. Этаж" },
            { "ROOM", "4. Помещение (Зона)" },
            { "BLOCK", "5. Блок (Узел)" },
            { "STAGE", "6. Стадия" },
            { "DISCIPLINES", "7. Разделы проекта" },
            { "DISCIPLINE", "8. Раздел" },
            { "CHAPTER", "9. Подраздел" },
            { "PART", "10. Часть" },
            { "GRP_BLD", "11. Группа зданий(сооружений)" },
            { "SYSTEM", "12. Система" },
            { "SUBSYSTEM", "13. Подсистема" },
            { "EQUIPMENT", "14. Оборудование" },
            { "LINE", "15. Линия" },
            { "GROUP_EQUIPMENT", "16. Группа оборудования" },
            { "UNION", "17. Штуцер(Люк)" },
            { "TERMINAL_BLOCK", "18. Клеммник" },
            { "TERMINAL", "19. Клемма" },
            { "PIPELINE", "20. Трубопровод" },
            { "PIPELINE_AXIS", "21. Участок" },
            { "CABLE", "22. Кабель" },
            { "CABLE_CORE", "23. Жила" },
            { "CONNECTION", "24. Подключение" },
            { "LINEAR_EQUIPMENT", "25. Линейное оборудование" },
            { "PIPLINE_FITTINGS", "26. Арматура" },
            { "SITUATION", "27. Ситуация" },
            { "SYSTEMS", "28. Системы" },
            { "CONSTRUCTIONS", "29. Конструкции" },
            { "DIFFERENT", "30. Разное" },
            { "EQUIPMENT_LINK", "31. Cсылка на оборудование" },
            { "FITTINGS_LINK", "32. Cсылка на арматуру" },
            { "LINEAR_EQUIPMENT_LINK", "33. Ссылка на линейное оборудование" },
            { "CONTROL", "34. Контроль" },
            { "SECTION", "35. Раздел" },
            { "SUBSECTION", "36. Подраздел" },
            { "GROUP", "37. Группа" },
            { "SUBGROUP", "38. Подгруппа" },
            { "TYPE", "39. Тип" },
            { "SUBTYPE", "40. Подтип" },
            { "ACCOUNT_ASM", "41. Учетная сборка" },
            { "ACCOUNT_OBJ", "42. Учетный объект" },
            { "SEGMENT", "43. Сегмент" },
            { "REDUCER", "44. Переход" },
            { "CABINET", "45. Шкаф" },
            { "DEVICE", "46. Прибор" },
            { "DEVICE_BLOCK", "47. Блок" },
            { "SECTION_CABINETS", "48. Секция" },
            { "BUS", "49. Шина" },
            { "ROOM_AREA", "50. Зона помещения" },
            { "BUILDING_BLOCK", "51. Блок зданий (Сооружений)" },
            { "MULTI_SECTION_BUILDING", "52. Многосекционное здание (Cооружение)" },
            { "SECTION_BUILDING", "53. Секция зданий (Сооружений)" },
            { "ROOM_FROUP", "54. Группа помещений" },
            { "ELEVATION", "55. Уровень" }
        };


        public const string CADLibParameter_BUILDINGS_STRUCT_LEVEL = "BUILDINGS_STRUCT_LEVEL";
        public const string CADLibParameterLABEL_BUILDINGS_STRUCT_LEVEL = "Уровень иерархии";

        public const string CADLibCategory_StructureData = "structure_data";

    }
}
