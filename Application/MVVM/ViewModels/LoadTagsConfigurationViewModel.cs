using Commands;
using Models;
using OSIsoft.AF.PI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
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

        //private bool _optionInputFile;
        //private bool _optionMissingSiteToBase;
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

        public bool OptionInputFile { get; set; } = true;

        public bool OptionMissingSiteToBase { get; set; }
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
                async o => await LoadAttributes());
        }
        #endregion

        #region Methods
        public async Task LoadAttributes()
        {
            _collectionTags.Clear();
            List<string> v_TagsNameList = new List<string>();
            //PIPointList v_FilteredPIPointList = new PIPointList();
            //PIPoint v_PIPointOut = null;

            if (OptionInputFile & !OptionMissingSiteToBase)
            {
                FilesManager.ParseInputFileToTagsList(ref v_TagsNameList);
                ReplicationManager.PIAttributesUpdateManager.LoadTagsAttributes(ReplicationManager.PIConnectionManager.PISourceServer, v_TagsNameList);

                PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGrid();
                
                // Test line by line
                //foreach (var tag in PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList)
                //{
                //    PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGridLineByLine(tag);
                //    OnPropertyChanged("Attributes");
                //}
            }
            else if (!OptionInputFile & OptionMissingSiteToBase)
            {
                // TODO : Call la méthode de mise à jour des tags site to base
                // On charge la liste des tags avec un instruments tags non vide depuis le serveur source
                //    List<PIPoint> v_AllPIPointsWithNoEmptyInstrumentTag = new List<PIPoint>(
                //        ReplicationManager.PISiteBaseManager.LoadAllPIPointsWithNoEmptyInstrumentTag(ReplicationManager.PIConnectionManager.PISourceServer));
                //    // On retire les tags qui existe sur le serveur destination
                //    v_FilteredPIPointList = new PIPointList(v_AllPIPointsWithNoEmptyInstrumentTag);
                //    foreach (var v_PIPoint in v_AllPIPointsWithNoEmptyInstrumentTag)
                //    {
                //        bool found = ReplicationManager.PISiteBaseManager.FilterExistingTags(v_PIPoint, ref v_PIPointOut, ref v_FilteredPIPointList, ReplicationManager.PIConnectionManager.PITargetServer);
                //        if (!found)
                //        {
                //            var v_TagAttributes = v_PIPointOut.GetAttributes();
                //            PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList.Add(v_TagAttributes);
                //            PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGridLineByLine(v_TagAttributes);
                //            OnPropertyChanged("Attributes");
                //            //OnPropertyChanged(nameof(Attributes));
                //        }
                //    }
                //}
                await PIReplicationManager.ReplicationManager.PISiteBaseManager.LoadDeltaTagsAttributesAsync(this);                
                //OnPropertyChanged(nameof(Attributes));

                // TODO: j'ai commenté temporairement pour la démo. il ya une exception quand je met en input un tag digital ==> pk ?
                //FilesManager.CreateTagsOutputFile(ReplicationManager.PIAttributesUpdateManager.AttributesTagsList, BackupType.SourceServerBackup);
            }
            #endregion
        }
    }
}
