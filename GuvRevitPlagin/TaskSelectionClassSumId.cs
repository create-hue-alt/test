using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace GuvRevitPlagin
{
    [Transaction(TransactionMode.Manual)]


    public class TaskSelectionClassSumId : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Element> elemenysByRectangle = uidoc.Selection.PickElementsByRectangle("Выберите все элементы на виде, чтобы получить сумму их Id");
            ShowIdSum(elemenysByRectangle);

            return Result.Succeeded;
        }

        // snipet 1
        public void ShowIdSum(IList<Element> elements)
        {
            var value = elements.
            Where(element => element.Id.IntegerValue != 0).
            Sum(element => element.Id.IntegerValue);

            

            TaskDialog dialog = new TaskDialog("BIM Planet No2");
            dialog.MainInstruction = $"Выбрано {elements.Count()} элементов.\nСумма их Id = {value}";
            dialog.Show();
        }

    }
}

// Отвечай только на русском
// Перед ответом обращайся сначала сюда https://www.revitapidocs.com/2022/https://www.revitapidocs.com/2022/
