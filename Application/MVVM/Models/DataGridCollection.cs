using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Models
{
    public class DataGridCollection
    {
        internal readonly CollectionViewSource CollectionViewSource = new CollectionViewSource();
        internal readonly ObservableCollection<PIPointGridFormat> CollectionTags = new ObservableCollection<PIPointGridFormat>();

        public DataGridCollection()
        {

        }

        internal void PopulateGrid()
        {
            foreach (var pipoint in PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList)
            {
                CollectionTags.Add(new PIPointGridFormat(
                    pipoint["tag"] as string,
                    pipoint["instrumenttag"] as string,
                    pipoint["pointtype"] as string,
                    pipoint["pointsource"] as string,
                    int.Parse(pipoint["location1"].ToString()),
                    float.Parse(pipoint["zero"].ToString()),
                    float.Parse(pipoint["typicalvalue"].ToString()),
                    float.Parse(pipoint["span"].ToString()),
                    int.Parse(pipoint["compressing"].ToString()),
                    float.Parse(pipoint["compdev"].ToString()),
                    float.Parse(pipoint["compdevpercent"].ToString()),
                    float.Parse(pipoint["compmin"].ToString()),
                    float.Parse(pipoint["excDev"].ToString()),
                    float.Parse(pipoint["excMin"].ToString()),
                    float.Parse(pipoint["excMax"].ToString()),
                    float.Parse(pipoint["excdevpercent"].ToString()),
                    float.Parse(pipoint["compdevpercent"].ToString()),
                    pipoint["datasecurity"].ToString(),
                    pipoint["ptsecurity"].ToString()
                    ));
            }
            CollectionViewSource.Source = CollectionTags;
        }

        internal void UpdateGrid()
        {
            CollectionTags.Clear();
            foreach (var pipoint in PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList)
            {
                CollectionTags.Add(new PIPointGridFormat(
                    pipoint["tag"] as string,
                    pipoint["instrumenttag"] as string,
                    pipoint["pointtype"] as string,
                    pipoint["pointsource"] as string,
                    int.Parse(pipoint["location1"].ToString()),
                    float.Parse(pipoint["zero"].ToString()),
                    float.Parse(pipoint["typicalvalue"].ToString()),
                    float.Parse(pipoint["span"].ToString()),
                    int.Parse(pipoint["compressing"].ToString()),
                    float.Parse(pipoint["compdev"].ToString()),
                    float.Parse(pipoint["compdevpercent"].ToString()),
                    float.Parse(pipoint["compmin"].ToString()),
                    float.Parse(pipoint["excDev"].ToString()),
                    float.Parse(pipoint["excMin"].ToString()),
                    float.Parse(pipoint["excMax"].ToString()),
                    float.Parse(pipoint["excdevpercent"].ToString()),
                    float.Parse(pipoint["compdevpercent"].ToString()),
                    pipoint["datasecurity"].ToString(),
                    pipoint["ptsecurity"].ToString()
                    ));
            }
            CollectionViewSource.Source = CollectionTags;
        }
    }
}

