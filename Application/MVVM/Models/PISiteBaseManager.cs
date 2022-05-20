using OSIsoft.AF.PI;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels;

namespace Models
{
    public class PISiteBaseManager
    {

        public async Task LoadDeltaTagsAttributesAsync(LoadTagsConfigurationViewModel p_LoadViewModel)
        {
            var v_AllPIPointsWithNoEmptyInstrumentTag = LoadAllPIPointsWithNoEmptyInstrumentTag();
            PIPointList v_FilteredPIPointList = new PIPointList(v_AllPIPointsWithNoEmptyInstrumentTag);
            await Task.Run(() =>
            {
                PIPoint v_ResultPIPoint = null;

                foreach (var v_PIPoint in v_AllPIPointsWithNoEmptyInstrumentTag)
                {
                    bool found = FilterExistingTags(v_PIPoint, ref v_ResultPIPoint, ref v_FilteredPIPointList);
                    if (!found)
                    {
                        var v_TagAttributes = v_ResultPIPoint.GetAttributes();
                        PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList.Add(v_TagAttributes);
                        PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGridLineByLine(v_TagAttributes);
                        p_LoadViewModel.OnPropertyChanged("Attributes");
                    }
                }
            });
        }

        public IEnumerable<PIPoint> LoadAllPIPointsWithNoEmptyInstrumentTag()
        {
            string query = "Name:=* AND instrumenttag:<>''";
            return PIPoint.FindPIPoints(PIReplicationManager.ReplicationManager.PIConnectionManager.PISourceServer, query, false);
        }

        public bool FilterExistingTags(PIPoint p_PIPoint, ref PIPoint v_ResultPIPoint, ref PIPointList p_PIPointList)
        {
            //PIPoint v_ResultPIPoint = null;
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
