# -*- coding: utf-8 -*-

from Autodesk.Revit.DB import *
from Autodesk.Revit.UI import *

def main():
    doc = __revit__.ActiveUIDocument.Document
    uidoc = __revit__.ActiveUIDocument

    family_name = "557_Радиатор_Секционный с боковым подключением_Универсальный_(MechEq)"

    try:
        # Поиск семейства
        family = None
        collector = FilteredElementCollector(doc).OfClass(Family)
        for f in collector:
            if f.Name == family_name:
                family = f
                break

        if not family:
            TaskDialog.Show("Ошибка", "Семейство не найдено!")
            return
            
        # Получаем любой тип семейства для проверки
        symbol_ids = family.GetFamilySymbolIds()
        symbol_id = next(iter(symbol_ids))
        symbol = doc.GetElement(symbol_id)
        
        # Определяем тип размещения
        placement_type = symbol.Family.FamilyPlacementType
        
        # АВТОМАТИЧЕСКОЕ определение доступных типов размещения
        type_mapping = {}
        available_types = []
        
        # Получаем все атрибуты FamilyPlacementType
        for attr_name in dir(FamilyPlacementType):
            if not attr_name.startswith("__") and not attr_name.endswith("__"):
                try:
                    attr_value = getattr(FamilyPlacementType, attr_name)
                    type_mapping[attr_value] = attr_name
                    available_types.append(attr_name)
                except:
                    pass
        
        # Создаем удобочитаемое описание
        russian_names = {
            "ViewBased": "на виде",
            "WorkPlaneBased": "на рабочей плоскости", 
            "OneLevelBased": "на одном уровне",
            "TwoLevelsBased": "на двух уровнях",
            "CurveBased": "на кривой",
            "CurveDrivenStructural": "на кривой (конструктивные)",
            "FaceBased": "на поверхности",  # Для старых версий
            "PlaceOnFace": "на поверхности"  # Для новых версий
        }
        
        # Определяем описание для текущего типа
        type_name = type_mapping.get(placement_type, "Неизвестный")
        placement_description = russian_names.get(type_name, type_name)
        
        # Показываем информацию о семействе
        message = "Имя: {}\nТип размещения: {} ({})\nFamilyPlacementType: {}\n\nДоступные типы в этой версии:\n{}".format(
            family.Name,
            placement_description,
            type_name,
            placement_type,
            ", ".join(available_types)
        )
        
        TaskDialog.Show("Информация о семействе", message)
        
    except Exception as e:
        TaskDialog.Show("Ошибка", str(e))

main()