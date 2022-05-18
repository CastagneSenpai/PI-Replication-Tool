using OSIsoft.AF.PI;
using System.Collections.Generic;

namespace Models
{
    public class PISiteBaseManager
    {
        public IEnumerable<PIPoint> LoadAllPIPointsWithNoEmpty(PIServer p_PIServer)
        {
            string query = "Name:=* AND instrumenttag:<>''";
            return PIPoint.FindPIPoints(p_PIServer, query, false);
            //PIPointList myPIPointList = new PIPointList(yolo);
            //return new PIPointList(yolo);
        }

        public PIPointList FilterExistingTags(IEnumerable<PIPoint> p_PIPointList, PIServer p_PIServer)
        {
            PIPointList list = new PIPointList(p_PIPointList);

            foreach (var v_Point in p_PIPointList)
            {
                if (PIPoint.TryFindPIPoint(p_PIServer, v_Point.Name, out _))
                {
                    if (!list.Remove(v_Point))
                    {
                        // NLOG
                    }
                }
            }
            list.LoadAttributes();
            return list;
        }
    }
}
