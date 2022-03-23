using Commands;
using Models;
using System.ComponentModel;
using System.Windows.Data;

namespace ViewModels
{
    public class PushTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;

        private readonly CollectionViewSource _collectionViewSource = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionViewSource;

        public ICollectionView Attributes
        {
            get
            {
                if (_collectionViewSource.View != null)
                {
                    _collectionViewSource.View.CurrentChanged += (sender, e) => PIPointGridFormat = _collectionViewSource.View.CurrentItem as PIPointGridFormat;
                    return _collectionViewSource?.View;
                }
                return null;
            }
        }

        private PIPointGridFormat _pipointgridformat = null;
        public PIPointGridFormat PIPointGridFormat
        {
            get => this._pipointgridformat;
            set
            {
                this._pipointgridformat = value;
                OnPropertyChanged(nameof(PIPointGridFormat));
            }
        }

        private string _destinationServer;
        public string DestinationServer
        {
            get
            {
                return _destinationServer;
            }
            set
            {
                _destinationServer = ReplicationManager.PIConnectionManager.PITargetServerName;
                OnPropertyChanged(nameof(DestinationServer));
            }
        }

        private readonly RelayCommand _buttonUpdateTags;
        public RelayCommand ButtonUpdateTags => _buttonUpdateTags;

        public PushTagsConfigurationViewModel()
        {
            DestinationServer = ReplicationManager.PIConnectionManager.PITargetServerName;

            _buttonUpdateTags = new RelayCommand(
                o => UpdateTagsAttributes());
        }

        public void UpdateTagsAttributes()
        {
            ReplicationManager.PIAttributesUpdateManager.UpdateTagsAttributes(
                ReplicationManager.PIConnectionManager.PISourceServer,
                ReplicationManager.PIConnectionManager.PITargetServer);

            PIReplicationManager.ReplicationManager.DataGridCollection.UpdateGrid();
            OnPropertyChanged(nameof(Attributes));
        }
    }
}
