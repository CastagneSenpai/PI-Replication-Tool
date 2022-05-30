using OSIsoft.AF.PI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Models
{
    public class PISiteBaseManager : Window
    {
        //public IEnumerable<PIPoint> AllPIPointsWithNoEmptyInstrumentTag;
        //List<PIPoint> v_FilteredPIPointList;

        //public async Task LoadDeltaTagsAttributesAsync(LoadTagsConfigurationViewModel p_LoadViewModel)
        public async Task<IEnumerable<PIPoint>> LoadDeltaTagsAttributesAsync()
        {
            //IEnumerable<PIPoint> AllPIPointsWithNoEmptyInstrumentTag = await LoadAllPIPointsWithNoEmptyInstrumentTagAsync();
            return await LoadAllPIPointsWithNoEmptyInstrumentTagAsync();
            //await Task.Run(() =>
            //{
            //    PIPointList v_FilteredPIPointList = new PIPointList(AllPIPointsWithNoEmptyInstrumentTag);
            //    //v_FilteredPIPointList = new List<PIPoint>(AllPIPointsWithNoEmptyInstrumentTag);
            //    PIPoint v_ResultPIPoint = null;

            //    foreach (var v_PIPoint in AllPIPointsWithNoEmptyInstrumentTag)
            //    {
            //        bool v_Found = FilterExistingTagsAsync(v_PIPoint, ref v_ResultPIPoint, ref v_FilteredPIPointList);
            //        if (!v_Found)
            //        {
            //            try
            //            {
            //                IDictionary<string, object> v_TagAttributes = v_ResultPIPoint.GetAttributes();
            //                PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList.Add(v_TagAttributes);
            //                PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGridLineByLine(v_TagAttributes);
            //                p_LoadViewModel.OnPropertyChanged("Attributes");
            //                //Dispatcher.Invoke(() => { 
            //                //    p_LoadViewModel.OnPropertyChanged("Attributes"); 
            //                //},DispatcherPriority.ContextIdle);
            //                //Application.Current.Dispatcher.Invoke(() => { p_LoadViewModel.OnPropertyChanged("Attributes"); }, DispatcherPriority.ContextIdle);
            //                //Collection.CollectionViewSource.View.Refresh();
            //                //p_LoadViewModel.OnPropertyChanged("Attributes");
            //            }
            //            catch (System.Exception)
            //            {
            //                // NLOG
            //            }
            //        }
            //    }
            //});
        }

        public async Task<IEnumerable<PIPoint>> LoadAllPIPointsWithNoEmptyInstrumentTagAsync()
        {
            string query = "Name:=* AND instrumenttag:<>''";
            return await PIPoint.FindPIPointsAsync(PIReplicationManager.ReplicationManager.PIConnectionManager.PISourceServer, query, false);
        }

        public bool FilterExistingTagsAsync(PIPoint p_PIPoint, ref PIPoint v_ResultPIPoint, ref PIPointList p_PIPointList)
        {
            //PIPoint v_ResultPIPoint = null;
            //return niketamere(v_ResultPIPoint, p_PIPointList);
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
