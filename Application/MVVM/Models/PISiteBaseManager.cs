using NLog;
using OSIsoft.AF.PI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Models
{
    public class PISiteBaseManager : Window
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public async Task<IEnumerable<PIPoint>> LoadDeltaTagsAttributesAsync()
        {
            return await LoadAllPIPointsWithNoEmptyInstrumentTagAsync();
        }

        public async Task<IEnumerable<PIPoint>> LoadAllPIPointsWithNoEmptyInstrumentTagAsync()
        {
            string query = "Name:=* AND instrumenttag:<>''";
            return await PIPoint.FindPIPointsAsync(PIReplicationManager.ReplicationManager.PIConnectionManager.PISourceServer, query, false);
        }

        public bool FilterExistingTags(PIPoint p_PIPoint, ref PIPoint p_ResultPIPoint, ref PIPointList p_PIPointList)
        {
            if (PIPoint.TryFindPIPoint(PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer, p_PIPoint.Name, out p_ResultPIPoint))
            {
                Logger.Debug($"Tag {p_PIPoint.Name} exist in both {PIReplicationManager.ReplicationManager.PIConnectionManager.PISourceServer} and {PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer} : it has NOT been added to the replication tag list");
                return p_PIPointList.Remove(p_PIPoint);
            }
            else
            {
                Logger.Info($"Tag {p_PIPoint.Name} exist in {PIReplicationManager.ReplicationManager.PIConnectionManager.PISourceServer} and not in {PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer} : it has been added to the replication tag list");
                return false;
            }
        }
    }
}
