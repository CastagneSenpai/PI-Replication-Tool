using OSIsoft.AF.Time;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Models
{
    public class DataGridCollection
    {
        #region Fields
        internal readonly CollectionViewSource CollectionViewSource = new CollectionViewSource();
        internal readonly ObservableCollection<PIPointGridFormat> CollectionTags = new ObservableCollection<PIPointGridFormat>();
        #endregion

        #region Constructors
        public DataGridCollection() { }
        #endregion

        #region Methods
        internal void AddToCollection(IDictionary<string, object> p_Tag, Constants.TagStatus? status = null, object p_CurrentValue = null, AFTime? p_CurrentTimestamp = null)
        {
            if (status.HasValue && p_CurrentValue != null && p_CurrentTimestamp.HasValue)
            {
                CollectionTags.Add(new PIPointGridFormat(
                    p_Tag["tag"] as string,
                    p_CurrentTimestamp.ToString(),
                    p_CurrentValue.ToString(),
                    p_Tag["instrumenttag"] as string,
                    p_Tag["pointtype"] as string,
                    p_Tag["pointsource"] as string,
                    int.Parse(p_Tag["location1"].ToString()),
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
            else
            {
                this.CollectionTags.Add(new PIPointGridFormat(
                        p_Tag["tag"] as string,
                        p_Tag["instrumenttag"] as string,
                        p_Tag["pointtype"] as string,
                        p_Tag["pointsource"] as string,
                        int.Parse(p_Tag["location1"].ToString()),
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
        }
        internal void PopulateGrid()
        {
            foreach (var pipoint in PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList)
            {
                AddToCollection(pipoint);
            }
            CollectionViewSource.Source = CollectionTags;
            CollectionViewSource.View.Refresh();
        }

        internal void PopulateGridLineByLine(IDictionary<string, object> toto)
        {
            AddToCollection(toto);

            CollectionViewSource.Source = CollectionTags;
        }

        internal void UpdateGrid()
        {
            CollectionTags.Clear();
            foreach (var pipoint in PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList)
            {
                AddToCollection(pipoint);
            }
            CollectionViewSource.Source = CollectionTags;
        }

        internal void UpdateGridStatus(IDictionary<string, object> pipoint, Constants.TagStatus status, object p_CurrentValue, AFTime p_CurrentTimestamp)
        {
            AddToCollection(pipoint, status, p_CurrentValue, p_CurrentTimestamp);
            CollectionViewSource.Source = CollectionTags;
        }
        #endregion Methods
    }
}

