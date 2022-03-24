using Commands;
using Models;
using System.ComponentModel;
using System.Windows.Data;

namespace ViewModels
{
    public class PushTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        #region Fields
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;

        private readonly CollectionViewSource _collectionViewSource = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionViewSource;
        private PIPointGridFormat _pipointgridformat = null;

        private string _destinationServer;
        #endregion

        #region Properties
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

        public PIPointGridFormat PIPointGridFormat
        {
            get => this._pipointgridformat;
            set
            {
                this._pipointgridformat = value;
                OnPropertyChanged(nameof(PIPointGridFormat));
            }
        }

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
        #endregion

        #region RelayCommands
        private readonly RelayCommand _buttonUpdateTags;
        private readonly RelayCommand _buttonPushTags;
        public RelayCommand ButtonUpdateTags => _buttonUpdateTags;
        public RelayCommand ButtonPushTags => _buttonPushTags;
        #endregion

        #region Constructor
        public PushTagsConfigurationViewModel()
        {
            DestinationServer = ReplicationManager.PIConnectionManager.PITargetServerName;

            _buttonUpdateTags = new RelayCommand(
                o => UpdateTagsAttributes());
            _buttonPushTags = new RelayCommand(
                o => PushTagsAttributes());
            // TODO: Vérifier qu'on ait bien cliquer sur le bouton update d'abord (?)
        }
        #endregion

        #region Methods
        public void UpdateTagsAttributes()
        {
            ReplicationManager.PIAttributesUpdateManager.UpdateTagsAttributes(
                ReplicationManager.PIConnectionManager.PISourceServer,
                ReplicationManager.PIConnectionManager.PITargetServer);

            PIReplicationManager.ReplicationManager.DataGridCollection.UpdateGrid();
            OnPropertyChanged(nameof(Attributes));
        }

        public void PushTagsAttributes()
        {
            PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.CreateAndPushTags(
                PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer);
        }
        #endregion
    }
}
