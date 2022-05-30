using NLog;
using OSIsoft.AF.PI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Models
{
    public class PISiteBaseManager : Window
    {
        static readonly Logger Logger = LogManager.GetLogger("PIReplicationToolLogger");
        public async Task<IEnumerable<PIPoint>> LoadDeltaTagsAttributesAsync()
        {
            return await LoadAllPIPointsWithNoEmptyInstrumentTagAsync();
        }

        public async Task<IEnumerable<PIPoint>> LoadAllPIPointsWithNoEmptyInstrumentTagAsync()
        {
            string query = "Name:=* AND instrumenttag:<>''";
            return await PIPoint.FindPIPointsAsync(PIReplicationManager.ReplicationManager.PIConnectionManager.PISourceServer, query, false);
        }

        public bool FilterExistingTagsAsync(PIPoint p_PIPoint, ref PIPoint v_ResultPIPoint, ref PIPointList p_PIPointList)
        {
            if (PIPoint.TryFindPIPoint(PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer, p_PIPoint.Name, out v_ResultPIPoint))
            {
                return p_PIPointList.Remove(p_PIPoint);
            }
            else
            {
                return false;
            }
        }
    }
}
