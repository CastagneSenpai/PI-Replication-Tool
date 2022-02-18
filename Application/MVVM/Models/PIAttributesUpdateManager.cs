using OSIsoft.AF.PI;
using System.Collections.Generic;

namespace Models
{
    public sealed class PIAttributesUpdateManager
    {
        public PIAttributesUpdateManager()
        {

        }

        public void LoadAttributes(PIServer p_PIServer, List<string> p_PITagNames, ref List<IDictionary<string, object>> p_ListAttributes)
        {
            List<PIPoint> v_PIPointList = new List<PIPoint>();            

            foreach (string piTagNames in p_PITagNames)
            {
                var v_PIPoint = PIPoint.FindPIPoint(p_PIServer, piTagNames);
                v_PIPointList.Add(v_PIPoint);
            }

            foreach (var v_PIPoint in v_PIPointList)
            {
                p_ListAttributes.Add(v_PIPoint.GetAttributes());
            }
        }
    }
}
