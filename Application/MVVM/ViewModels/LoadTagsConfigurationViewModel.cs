using Commands;
using Models;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
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
        //public ICollectionView Attributes
        //{
        //    get
        //    {
        //        if (_collectionViewSource.View != null)
        //        {
        //            _collectionViewSource.View.CurrentChanged += (sender, e) => PIPointGridFormat = _collectionViewSource.View.CurrentItem as PIPointGridFormat;
        //            return _collectionViewSource?.View;
        //        }
        //        return null;
        //    }
        //}
        public ObservableCollection<PIPointGridFormat> Attributes
        {
            get
            {
                return PIReplicationManager.ReplicationManager.DataGridCollection.CollectionTags;
            }
            set
            {
                SetProperty(ref PIReplicationManager.ReplicationManager.DataGridCollection.CollectionTags, value);
                OnPropertyChanged(nameof(Attributes));
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

        public string OptionLocalInputFileContent { get; set; } = "Tags from local input file (" + ConfigurationManager.AppSettings["InputPath"] + ConfigurationManager.AppSettings["InputFileName"] + ")";
        #endregion

        #region RelayCommands
        private readonly AsyncCommand _buttonLoadTags;
        public IAsyncCommand ButtonLoadTags => _buttonLoadTags;
        #endregion

        #region Constructor
        public LoadTagsConfigurationViewModel()
        {
            SourceServer = ReplicationManager.PIConnectionManager.PISourceServerName;
            _buttonLoadTags = new AsyncCommand(LoadAttributesAsync);
        }
        #endregion

        #region Methods
        public async Task LoadAttributesAsync()
        {
            _collectionTags.Clear();
            List<string> v_TagsNameList = new List<string>();
            //PIPointList v_FilteredPIPointList = new PIPointList();
            //PIPoint v_PIPointOut = null;

            if (OptionInputFile & !OptionMissingSiteToBase)
            {
                FilesManager.ParseInputFileToTagsList(ref v_TagsNameList);
                ReplicationManager.PIAttributesUpdateManager.LoadTagsAttributes(ReplicationManager.PIConnectionManager.PISourceServer, v_TagsNameList);

                await Task.Run(() =>
                {
                    foreach (var v_PIPoint in PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList)
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGridLineByLine(v_PIPoint);
                        });
                    }
                });
                //PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGrid();
                //OnPropertyChanged("Attributes");

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
                //await PIReplicationManager.ReplicationManager.PISiteBaseManager.LoadDeltaTagsAttributesAsync(this);
                IEnumerable<PIPoint> AllPIPointsWithNoEmptyInstrumentTag = await PIReplicationManager.ReplicationManager.PISiteBaseManager.LoadDeltaTagsAttributesAsync();

                PIPointList v_FilteredPIPointList = new PIPointList(AllPIPointsWithNoEmptyInstrumentTag);
                //v_FilteredPIPointList = new List<PIPoint>(AllPIPointsWithNoEmptyInstrumentTag);

                // TODO gerer le cas list null
                PIPoint v_ResultPIPoint = null;

                await Task.Run(() =>
                {
                    foreach (var v_PIPoint in AllPIPointsWithNoEmptyInstrumentTag)
                    {
                        bool v_Found = PIReplicationManager.ReplicationManager.PISiteBaseManager.FilterExistingTagsAsync(v_PIPoint, ref v_ResultPIPoint, ref v_FilteredPIPointList);
                        if (!v_Found)
                        {
                            try
                            {
                                IDictionary<string, object> v_TagAttributes = v_PIPoint.GetAttributes();
                                if (v_PIPoint.PointType.Equals(PIPointType.Digital))
                                {
                                    // NLOG
                                }
                                else
                                {
                                    PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList.Add(v_TagAttributes);
                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGridLineByLine(v_TagAttributes);
                                    });
                                    //Dispatcher.CurrentDispatcher.Invoke(() =>
                                    //{
                                    //    PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGridLineByLine(v_TagAttributes);
                                    //}, DispatcherPriority.ContextIdle);
                                }
                                //_collectionViewSource.View.Refresh();
                                //OnPropertyChanged("Attributes");


                                //p_LoadViewModel.OnPropertyChanged("Attributes");
                                //Dispatcher.CurrentDispatcher.Invoke(() =>
                                //{
                                //    OnPropertyChanged(nameof(Attributes));
                                //}, DispatcherPriority.ContextIdle);

                                //Application.Current.Dispatcher.Invoke(() => { OnPropertyChanged("Attributes"); }, DispatcherPriority.ContextIdle);

                                // TODO: j'ai commenté temporairement pour la démo. il ya une exception quand je met en input un tag digital ==> pk ?
                                //FilesManager.CreateTagsOutputFile(ReplicationManager.PIAttributesUpdateManager.AttributesTagsList, BackupType.SourceServerBackup);
                            }
                            catch (System.Exception)
                            {
                                // NLOG
                            }
                        }
                    }
                });
            }
        }
        #endregion
    }
}
