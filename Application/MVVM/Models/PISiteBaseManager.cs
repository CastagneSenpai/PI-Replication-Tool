using OSIsoft.AF.PI;
using System.Collections.Generic;

namespace Models
{
    public class PISiteBaseManager
    {
        public IEnumerable<PIPoint> LoadAllPIPointsWithNoEmptyInstrumentTag(PIServer p_PIServer)
        {
            string query = "Name:=* AND instrumenttag:<>''";
            return PIPoint.FindPIPoints(p_PIServer, query, false);
        }

        public void FilterExistingTags(PIPoint p_PIPoint, ref PIPointList p_PIPointList, PIServer p_PIServer)
        {
            PIPoint v_ResultPIPoint = null;
            if (PIPoint.TryFindPIPoint(p_PIServer, p_PIPoint.Name, out v_ResultPIPoint))
            {
                if (!p_PIPointList.Remove(v_ResultPIPoint))
                {
                    // NLOG
                }
            }
            //p_PIPointList.LoadAttributes();
        }
    }
}
