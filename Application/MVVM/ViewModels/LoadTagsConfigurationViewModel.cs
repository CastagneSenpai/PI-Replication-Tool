using Commands;
using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace ViewModels
{
    public class LoadTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        #region Fields
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;

        private readonly CollectionViewSource _collectionViewSource = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionViewSource;
        private readonly ObservableCollection<PIPointGridFormat> _collectionTags = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionTags;

        private PIPointGridFormat _pipointgridformat = null;

        private string _sourceServer;
        private int _choiceRadiotButton;
        #endregion

        #region Properties

        public int ChoiceRadioButton
        {
            get
            {
                return _choiceRadiotButton;
            }
            set
            {
                _choiceRadiotButton = value;
            }
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

        public PIPointGridFormat PIPointGridFormat
        {
            get => this._pipointgridformat;
            set
            {
                _pipointgridformat = value;
                OnPropertyChanged(nameof(PIPointGridFormat));
            }
        }

        public string SourceServer
        {
            get => _sourceServer;
            set
            {
                _sourceServer = ReplicationManager.PIConnectionManager.PISourceServerName;
                OnPropertyChanged(nameof(SourceServer));
            }
        }
        #endregion

        #region RelayCommands
        private readonly RelayCommand _buttonLoadTags;
        public RelayCommand ButtonLoadTags => _buttonLoadTags;
        #endregion

        #region Constructor
        public LoadTagsConfigurationViewModel()
        {
            SourceServer = ReplicationManager.PIConnectionManager.PISourceServerName;
            _buttonLoadTags = new RelayCommand(
                o => LoadAttributes());
        }
        #endregion

        #region Methods
        void LoadAttributes()
        {
            List<string> v_TagsNameList = new List<string>();
            FilesManager.ParseInputFileToTagsList(ref v_TagsNameList);

            _collectionTags.Clear();

            ReplicationManager.PIAttributesUpdateManager.LoadTagsAttributes(ReplicationManager.PIConnectionManager.PISourceServer, v_TagsNameList);

            PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGrid();
            OnPropertyChanged("Attributes");

            // TODO: j'ai commenté temporairement pour la démo. il ya une exception quand je met en input un tag digital ==> pk ?
            //FilesManager.CreateTagsOutputFile(ReplicationManager.PIAttributesUpdateManager.AttributesTagsList, BackupType.SourceServerBackup);

            PIReplicationManager.Logger.Info("Ca marche !");
        }
        #endregion Methods
    }
}
