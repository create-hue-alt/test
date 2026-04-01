using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace GuvRevitPlagin
{
    [Transaction(TransactionMode.Manual)]

    public class SumRoomsSquare : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                var rooms = PickRooms(uidoc, doc);
                ShowRoomsArea(rooms);
                return Result.Succeeded;
            }

            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private static BuiltInCategory GetBuiltInCategoryById(int id)
        {
            foreach (BuiltInCategory category in Enum.GetValues(typeof(BuiltInCategory)))
            {
                if ((int)category == id)
                {
                    return category;
                }
            }

            throw new ArgumentException($"No BuiltInCategory found for id: {id}.");            
        }

        private static object GetParametrValue(Parameter parameter)
        {
            if (parameter == null) return null;

            switch (parameter.StorageType)
            {
                case StorageType.String:
                    return parameter.AsString();
                case StorageType.Integer:
                    return parameter.AsInteger();
                case StorageType.Double:
                    return UnitUtils.ConvertFromInternalUnits(
                        parameter.AsDouble(), parameter.GetUnitTypeId());
                case StorageType.ElementId:
                    return parameter.AsElementId();
                default:
                    return null;
            }
        }

        private static IList<Element> PickRooms(UIDocument uidoc, Document doc)
        {
            var reference = uidoc.Selection.PickObjects(
                ObjectType.Element,
                new RoomSelectionFilter(),
                "Выделите помещения");

            return reference.
                Select(r => doc.GetElement(r)).
                ToList();
        }

        private static void ShowRoomsArea(IList<Element> rooms)
        {
            double areaSum = rooms
                .Select(room => GetParametrValue(room.get_Parameter(BuiltInParameter.ROOM_AREA)))
                .OfType<double>()
                .Sum();

            int areaSumRound = (int)Math.Round(areaSum);

            TaskDialog dialog = new TaskDialog("BIM Planet No2");
            dialog.MainInstruction = $"Выбрано {rooms.Count()} помещений.\nОбщая площадь = {areaSumRound} м2";
            dialog.Show();
        }
    }

    public class RoomSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if(element.Category == null) return false;

            foreach (BuiltInCategory category in Enum.GetValues(typeof(BuiltInCategory)))
            {
                if ((int)category == element.Category.Id.IntegerValue)
                    return category == BuiltInCategory.OST_Rooms;
            }
            return true;
        }

        public bool AllowReference(Reference refer, XYZ point) => false;
    }
}
