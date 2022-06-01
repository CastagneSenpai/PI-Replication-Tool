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
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;
        private readonly ObservableCollection<PIPointGridFormat> _collectionTags = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionTags;
        private PIPointGridFormat _pipointgridformat = null;
        private string _sourceServer;
        private double _currentProgress = 0;
        private double _tagProgress = 0;
        private double _totalProgress = 0;
        private bool _taskBarVisibility = false;
        private bool _isLoadTagButtonAvailable = true;
        #endregion

        #region Properties
        public double CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }
        public double TagProgress
        {
            get => _tagProgress;
            set
            {
                _tagProgress = value;
                OnPropertyChanged(nameof(TagProgress));
            }
        }
        public double TotalProgress
        {
            get => _totalProgress;
            set
            {
                _totalProgress = value;
                OnPropertyChanged(nameof(TotalProgress));
            }
        }
        public bool TaskBarVisibility
        {
            get => _taskBarVisibility;
            set
            {
                if (_taskBarVisibility != value)
                    _taskBarVisibility = value;
            }
        }
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
        public bool IsLoadTagButtonAvailable 
        {
            get => _isLoadTagButtonAvailable;
            set
            {
                SetProperty(ref _isLoadTagButtonAvailable, value);
                OnPropertyChanged(nameof(IsLoadTagButtonAvailable));
            }

        }
        #endregion

        #region RelayCommands
        private readonly AsyncCommand _buttonLoadTags;
        public IAsyncCommand ButtonLoadTags => _buttonLoadTags;
        #endregion

        #region Constructor
        public LoadTagsConfigurationViewModel()
        {
            SourceServer = ReplicationManager.PIConnectionManager.PISourceServerName;
            _buttonLoadTags = new AsyncCommand(LoadAttributesAsync, IsSourceServerSelected);
        }
        #endregion

        #region Methods
        public async Task LoadAttributesAsync()
        {
            Logger.Info("Call method LoadTagsConfigurationViewModel.LoadAttributesAsync");

            // Disable load button during loading process
            this.IsLoadTagButtonAvailable = false;

            _collectionTags.Clear();
            List<string> v_TagsNameList = new List<string>();

            if (OptionInputFile & !OptionMissingSiteToBase)
            {
                Logger.Info("Option \"Input File\" selected");
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
                Logger.Info("Option \"Missing tags from site to base\" selected");

                TaskBarVisibility = true; // Display task bar
                IEnumerable<PIPoint> AllPIPointsWithNoEmptyInstrumentTag = await PIReplicationManager.ReplicationManager.PISiteBaseManager.LoadDeltaTagsAttributesAsync();
                PIPointList v_FilteredPIPointList = new PIPointList(AllPIPointsWithNoEmptyInstrumentTag);

                // TODO gerer le cas list null
                PIPoint v_ResultPIPoint = null;

                TotalProgress = v_FilteredPIPointList.Count;
                var progress = new Progress<double>(v_currentPercent =>
                {
                    CurrentProgress = (double)v_currentPercent / TotalProgress * 100;
                });

                await LoadTagsMissingSiteToBase(AllPIPointsWithNoEmptyInstrumentTag, v_ResultPIPoint, v_FilteredPIPointList, progress);
            }

            // Creation of source backup file
            FilesManager.CreateTagsOutputFile(ReplicationManager.PIAttributesUpdateManager.AttributesTagsList, BackupType.SourceServerBackup);

            // Re-enable load button
            this.IsLoadTagButtonAvailable = true;

            Logger.Info("End method LoadTagsConfigurationViewModel.LoadAttributesAsync");
        }

        // TODO Déplacer dans la classe SiteBaseManager
        async Task LoadTagsMissingSiteToBase(IEnumerable<PIPoint> p_AllPIPointsWithNoEmptyInstrumentTag, PIPoint p_ResultPIPoint, PIPointList p_FilteredPIPointList, IProgress<double> p_progress)
        {
            await Task.Run(() =>
            {
                foreach (var v_PIPoint in p_AllPIPointsWithNoEmptyInstrumentTag)
                {
                    TagProgress++;
                    bool v_Found = PIReplicationManager.ReplicationManager.PISiteBaseManager.FilterExistingTags(v_PIPoint, ref p_ResultPIPoint, ref p_FilteredPIPointList);
                    if (!v_Found)
                    {
                        try
                        {
                            IDictionary<string, object> v_TagAttributes = v_PIPoint.GetAttributes();
                            if (v_PIPoint.PointType.Equals(PIPointType.Digital))
                            {
                                Logger.Debug($"{v_PIPoint.Name} - Digital point taken into account.");
                            }
                            else
                            {
                                Logger.Debug($"{v_PIPoint.Name} - Numerical point taken into account.");
                            }

                            PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList.Add(v_TagAttributes);
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGridLineByLine(v_TagAttributes);
                                p_progress.Report(TagProgress);
                            });
                        }
                        catch (Exception exc)
                        {
                            Logger.Debug($"{v_PIPoint.Name} - Digital point taken into account. {exc.Message}");
                        }
                    }
                }
            });
        }

        private bool IsSourceServerSelected()
        {
            return !string.IsNullOrEmpty(ReplicationManager.PIConnectionManager.PISourceServerName);
        }
        #endregion
    }
}
