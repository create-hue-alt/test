using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuvRevitPlagin
{
    [Transaction(TransactionMode.Manual)]
    internal class Task_3_3_12 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var log = new StringBuilder();
            int symbolsSum = 0;
            int instancesSum = 0;
        
            List<FamilyInstance> filter = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .ToList();

            HashSet<ElementId> processedFamilies = new HashSet<ElementId>();

            foreach (FamilyInstance instance in filter)
            {
                Family family = instance.Symbol.Family;

                ElementId familyId = family.Id;

                if (processedFamilies.Contains(familyId)) continue;
                                
                ISet<ElementId> symbolsIds = family.GetFamilySymbolIds();

                if (familyId.IntegerValue > 100000)
                {
                    foreach (ElementId id in symbolsIds) symbolsSum += id.IntegerValue;
                }
                else
                {
                    foreach(FamilyInstance instance1 in filter)
                    {
                        if (instance1.Symbol.Family.Id == familyId)
                        {
                            instancesSum += instance1.Id.IntegerValue;
                        }
                    }
                }

                processedFamilies.Add(familyId);
            }

            TaskDialog.Show("Результат", $"Сумма id: {symbolsSum + instancesSum}");

            return Result.Succeeded;
        }
    }
}

