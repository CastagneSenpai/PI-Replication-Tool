using NLog;
using OSIsoft.AF.Time;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Models
{
    public class DataGridCollection
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Fields
        internal readonly CollectionViewSource CollectionViewSource = new CollectionViewSource();
        internal ObservableCollection<PIPointGridFormat> CollectionTags = new ObservableCollection<PIPointGridFormat>();
        #endregion

        #region Constructors
        public DataGridCollection() { }
        #endregion

        #region Methods
        internal void AddToCollection(IDictionary<string, object> p_Tag, Constants.TagStatus? status = null, object p_CurrentValue = null, AFTime? p_CurrentTimestamp = null)
        {
            Logger.Debug($"Call method DataGridCollection.AddToCollection.");
            
            // Update grid for push mode
            if (status.HasValue && p_CurrentValue != null && p_CurrentTimestamp.HasValue)
            {
                CollectionTags.Add(new PIPointGridFormat(
                p_Tag["tag"] as string,
                p_CurrentTimestamp.ToString(),
                p_CurrentValue.ToString(),
                p_Tag["instrumenttag"] as string,
                p_Tag["pointsource"] as string,
                int.Parse(p_Tag["location1"].ToString()),
                p_Tag["pointtype"].ToString(),
                p_Tag["digitalset"] as string,
                float.Parse(p_Tag["zero"].ToString()),
                float.Parse(p_Tag["typicalvalue"].ToString()),
                float.Parse(p_Tag["span"].ToString()),
                int.Parse(p_Tag["compressing"].ToString()),
                float.Parse(p_Tag["compdev"].ToString()),
                float.Parse(p_Tag["compmin"].ToString()),
                float.Parse(p_Tag["compdevpercent"].ToString()),
                float.Parse(p_Tag["excDev"].ToString()),
                float.Parse(p_Tag["excMin"].ToString()),
                float.Parse(p_Tag["excMax"].ToString()),
                float.Parse(p_Tag["excdevpercent"].ToString()),
                float.Parse(p_Tag["compdevpercent"].ToString()),
                p_Tag["datasecurity"].ToString(),
                p_Tag["ptsecurity"].ToString(),
                status.Value
            ));
            }
            // Update grid for load mode
            else
            {
                this.CollectionTags.Add(new PIPointGridFormat(
                        p_Tag["tag"] as string,
                        p_Tag["instrumenttag"] as string,
                        p_Tag["pointsource"] as string,
                        int.Parse(p_Tag["location1"].ToString()),
                        p_Tag["pointtype"].ToString(),
                        p_Tag["digitalset"] as string,
                        float.Parse(p_Tag["zero"].ToString()),
                        float.Parse(p_Tag["typicalvalue"].ToString()),
                        float.Parse(p_Tag["span"].ToString()),
                        int.Parse(p_Tag["compressing"].ToString()),
                        float.Parse(p_Tag["compdev"].ToString()),
                        float.Parse(p_Tag["compmin"].ToString()),
                        float.Parse(p_Tag["compdevpercent"].ToString()),
                        float.Parse(p_Tag["excDev"].ToString()),
                        float.Parse(p_Tag["excMin"].ToString()),
                        float.Parse(p_Tag["excMax"].ToString()),
                        float.Parse(p_Tag["excdevpercent"].ToString()),
                        float.Parse(p_Tag["compdevpercent"].ToString()),
                        p_Tag["datasecurity"].ToString(),
                        p_Tag["ptsecurity"].ToString()
                        ));
            }
            Logger.Debug($"End method DataGridCollection.AddToCollection.");
        }
        internal void PopulateGrid()
        {
            foreach (var v_PIPoint in PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList)
            {
                AddToCollection(v_PIPoint);
            }
        }

        internal void PopulateGridLineByLine(IDictionary<string, object> p_Dictionary)
        {
            AddToCollection(p_Dictionary);
        }

        internal void UpdateGrid()
        {
            Logger.Debug($"Call method DataGridCollection.UpdateGrid.");
            CollectionTags.Clear();
            foreach (var pipoint in PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList)
            {
                AddToCollection(pipoint);
            }
            Logger.Debug($"End method DataGridCollection.UpdateGrid.");
        }

        internal void UpdateGridStatus(IDictionary<string, object> pipoint, Constants.TagStatus status, object p_CurrentValue, AFTime p_CurrentTimestamp)
        {
            Logger.Debug($"Call method DataGridCollection.UpdateGridStatus.");
            AddToCollection(pipoint, status, p_CurrentValue, p_CurrentTimestamp);
            Logger.Debug($"End method DataGridCollection.UpdateGridStatus.");
        }
        #endregion
    }
}

