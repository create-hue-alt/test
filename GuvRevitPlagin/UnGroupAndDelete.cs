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

namespace GUV
{
    [Transaction(TransactionMode.Manual)]
    //[Transaction(TransactionMode.ReadOnly)]

    public class GuvRevitCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //UIApplication uiApp = commandData.Application;
            //Application app = uiApp.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction t = new Transaction(doc, "GUV_Plagin_Ungroup"))
            {
                t.Start();

                // 1. Разгруппировка
                var collector = new FilteredElementCollector(doc).
                    OfClass(typeof(Group)).
                    Cast<Group>().
                    ToList();

                foreach (var group in collector)
                {

                    if (group == null || group.IsValidObject == false)
                        continue;
                    try
                    {
                        if (group.Pinned)
                            group.Pinned = false;

                        group.UngroupMembers();
                    }
                    catch
                    {
                    }
                }
                // 2. удаление пустых типов
                List<GroupType> groupTypes = new FilteredElementCollector(doc).
                    OfClass(typeof(GroupType)).
                    Cast<GroupType>().
                    ToList();

                foreach (var type in groupTypes)
                {

                    if (type == null || type.IsValidObject == false)
                        continue;

                    try
                    {

                        if (type.Groups.IsEmpty)
                        {
                            doc.Delete(type.Id);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                t.Commit();
            }
            return Result.Succeeded;
        }
            
    }
}     

