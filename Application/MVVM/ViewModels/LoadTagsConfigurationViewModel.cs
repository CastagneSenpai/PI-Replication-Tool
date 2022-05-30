using Commands;
using Models;
using NLog;
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
        static readonly Logger Logger = LogManager.GetLogger("PIReplicationToolLogger");
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;
        private readonly ObservableCollection<PIPointGridFormat> _collectionTags = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionTags;
        private PIPointGridFormat _pipointgridformat = null;
        private string _sourceServer;
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
            Logger.Info("Call method LoadTagsConfigurationViewModel.LoadAttributesAsync");

            _collectionTags.Clear();
            List<string> v_TagsNameList = new List<string>();

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
            }
            else if (!OptionInputFile & OptionMissingSiteToBase)
            {
                IEnumerable<PIPoint> AllPIPointsWithNoEmptyInstrumentTag = await PIReplicationManager.ReplicationManager.PISiteBaseManager.LoadDeltaTagsAttributesAsync();
                PIPointList v_FilteredPIPointList = new PIPointList(AllPIPointsWithNoEmptyInstrumentTag);

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
                                }                                
                            }
                            catch (System.Exception)
                            {
                                // NLOG
                            }
                        }
                    }
                });
            }
            Logger.Info("End method LoadTagsConfigurationViewModel.LoadAttributesAsync");
        }
        #endregion
    }
}
