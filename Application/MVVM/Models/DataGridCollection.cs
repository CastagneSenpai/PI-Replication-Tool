using Models;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace PI_Replication_Tool.MVVM.Models
{
    public class DataGridCollection
    {
        internal readonly CollectionViewSource _collectionViewSource = new CollectionViewSource();
        internal readonly ObservableCollection<PIPointGridFormat> _collectionTags = new ObservableCollection<PIPointGridFormat>();

        public DataGridCollection()
        {

        }
    }
}
