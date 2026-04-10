using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GuvRevitPlagin
{
    [Transaction(TransactionMode.Manual)]
    internal class Task_3_4 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            StringBuilder log = new StringBuilder();
            double sumTotal = 0;
            double sumCurrent = 0;

            List<RoofBase> filterRoofs = new FilteredElementCollector(doc)
                .OfClass(typeof(RoofBase))
                .WhereElementIsNotElementType()
                .Cast<RoofBase>()
                .ToList();

            foreach (RoofBase roof in filterRoofs)
            {
                if (roof.SlabShapeEditor != null &&  roof.SlabShapeEditor.IsEnabled)
                {
                    var vertices = roof.SlabShapeEditor.SlabShapeVertices;

                    foreach (SlabShapeVertex vertex in vertices)
                    {
                        XYZ xyz = vertex.Position;

                        var x = UnitUtils.ConvertFromInternalUnits(xyz.X, UnitTypeId.Millimeters);
                        var y = UnitUtils.ConvertFromInternalUnits(xyz.Y, UnitTypeId.Millimeters);
                        var z = UnitUtils.ConvertFromInternalUnits(xyz.Z, UnitTypeId.Millimeters);

                        sumCurrent = x + y + z;

                        sumTotal += sumCurrent;

                        log.AppendLine($"X.intUnits: {xyz.X}, Y.intUnits: {xyz.Y}, Z.intUnits: {xyz.Z}\n" +
                            $"-------------------------------------------------------------------\n" +
                            $"X.Millimeters: {x}, Y.Millimeters: {y}, Z.Millimeters: {z}\n" +
                            $"-------------------------------------------------------------------\n\n\n\n");
                    }
                }

            }

            log.AppendLine($"Общая сумма: {sumTotal}");
            string logPath = @"C:\Users\GUV\Documents\000_GUV_Local\RevitLog.txt";

            File.WriteAllText(logPath, log.ToString());

            TaskDialog.Show("Скрипт отработал!", $"Файл сохранен по пути: {logPath}");
            
            return Result.Succeeded;
        }
    }
}
