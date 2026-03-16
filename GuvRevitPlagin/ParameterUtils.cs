using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace GuvRevitPlagin
{
    internal class ParameterUtils
    {
        public static double GetDouble(Parameter p)
        {
            if (p == null || p.StorageType != StorageType.Double)
                return 0.0;

            return UnitUtils.ConvertFromInternalUnits(
                p.AsDouble(),
                p.GetUnitTypeId());
        }

        public static int? GetInt(Parameter p)
        {
            if (p == null || p.StorageType != StorageType.Integer)
                return null;

            return p.AsInteger();
        }

        public static string GetString(Parameter p)
        {
            if (p == null || p.StorageType != StorageType.String)
                return null;

            return p.AsString();
        }

        public static ElementId GetElemetnId(Parameter p)
        {
            if (p == null || p.StorageType != StorageType.ElementId)
                return null;

            return p.AsElementId();
        }
    }
}
