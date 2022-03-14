using Commands;
using Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace ViewModels
{
    public class PushTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;

        private readonly CollectionViewSource _collectionViewSource = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionViewSource;
        private readonly ObservableCollection<PIPointGridFormat> _collectionTags = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionTags;

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

        //private readonly RelayCommand _buttonPushTags;
        //public RelayCommand ButtonPushTags => _buttonPushTags;

        public PushTagsConfigurationViewModel()
        {
            DestinationServer = ReplicationManager.PIConnectionManager.PITargetServerName;

            _buttonUpdateTags = new RelayCommand(
                o => this.ReplicationManager.PIAttributesUpdateManager.UpdateTagsAttributes(
                    ReplicationManager.PIConnectionManager.PISourceServer,
                    ReplicationManager.PIConnectionManager.PITargetServer),
                o => true);

            //_buttonPushTags = new RelayCommand(
            //    o => 
            //    o => true);
        }

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


    }
}
