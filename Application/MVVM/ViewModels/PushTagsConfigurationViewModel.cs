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
        private readonly RelayCommand _buttonRefresh;
        public RelayCommand ButtonUpdateTags => _buttonUpdateTags;
        public RelayCommand ButtonPushTags => _buttonPushTags;
        public RelayCommand ButtonRefresh => _buttonRefresh;
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
            _buttonRefresh = new RelayCommand(
                o => UpdateRowsUsingCurrentValues());
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
            FilesManager.CreateTagsOutputFile(ReplicationManager.PIAttributesUpdateManager.AttributesTagsList, BackupType.TargetServerBackup);
            PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.CreateAndPushTags(
                PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer);
        }

        public void UpdateRowsUsingCurrentValues()
        {
            PIReplicationManager.ReplicationManager.DataGridCollection.CollectionTags.Clear();
            var v_tagList = PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList;
            v_tagList.ForEach(v_AttributesTags =>
            {
                PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.GetCurrentValues(PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer, v_AttributesTags);
                OnPropertyChanged(nameof(Attributes));
            });

        }
        #endregion
    }
}
