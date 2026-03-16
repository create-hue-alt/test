using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Xml.Schema;

namespace GuvRevitPlagin
{
    [Transaction(TransactionMode.Manual)]
    //[Transaction(TransactionMode.ReadOnly)]

    public class GuvRevitCommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Application app = uiApp.Application;            
            //UIDocument uidoc = commandData.Application.ActiveUIDocument;
            //Document doc = uidoc.Document;

            var log = new StringBuilder();
            int sum = 0;


            // Мой код

            string[] files = Directory.GetFiles
                (@"C:\Users\GUV\Documents\000_GUV_Local\IronPython_3305_V_UIApplication_2020","*.rvt");

            Document pervDoc = null;

            foreach (string file in files)
            {
                uiApp.OpenAndActivateDocument(file);

                UIDocument uidoc = uiApp.ActiveUIDocument;

                Document doc = uidoc.Document;

                ElementId activeView = uidoc.ActiveView.Id;

                int collector  = new FilteredElementCollector(doc, activeView).Count();

                sum += collector;

                if (pervDoc != null)
                { pervDoc.Close(false); }

                pervDoc = doc;
                
            }

            //string logPath = @"C:\Users\GUV\Documents\000_GUV_Local\RevitLog.txt";

            //File.WriteAllText(logPath, log.ToString());

            TaskDialog.Show("Result", $"Файл сохранен\n Sum  = {sum}");    

            return Result.Succeeded;

           
        }
    }
}
