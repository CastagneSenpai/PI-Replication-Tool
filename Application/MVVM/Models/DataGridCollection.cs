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
    }
}
