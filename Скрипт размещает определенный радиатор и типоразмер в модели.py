# -*- coding: utf-8 -*-

from Autodesk.Revit.DB import *
from Autodesk.Revit.UI import *
from System.Collections.Generic import List

# Получаем текущий документ и активный вид
doc = __revit__.ActiveUIDocument.Document  # type: Document
uidoc = __revit__.ActiveUIDocument  # type: UIDocument
active_view = doc.ActiveView  # type: View
level = active_view.GenLevel

family_name = "557_Радиатор_Секционный с боковым подключением_Универсальный_(MechEq)"  # type: str
symbol_name = "350"

# Поиск семейства в документе
family = None  # type: Family
collector = FilteredElementCollector(doc).OfClass(Family)
for f in collector:
    if f.Name == family_name:
        family = f
# print(family.Name)

symbol_ids = family.GetFamilySymbolIds()    # type: ISet[ElementId]

for id in symbol_ids:
    if symbol_name == doc.GetElement(id).LookupParameter('Type Name').AsString():
        symbol = doc.GetElement(id)

# print(symbol, type(symbol), symbol.LookupParameter('Type Name').AsString())

point = uidoc.Selection.PickPoint('Выберите точку для размещения семейства')    #type: XYZ

try:
    with Transaction(doc, 'Размещение сеемйства') as t:
        t.Start()
        # Для OneLevelBased используем метод с указанием уровня
        family_instance = doc.Create.NewFamilyInstance(
            point,
            symbol,
            level,
            0
        )   #type: FamilyInstance
        t.Commit()
    # Выделяем созданный элемент
    uidoc.Selection.SetElementIds(List[ElementId]([family_instance.Id]))

except Exception as e:
    print(str(e))