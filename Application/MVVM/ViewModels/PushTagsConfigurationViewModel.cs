using Commands;
using Models;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using NLog;

namespace ViewModels
{
    public class PushTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        #region Fields
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;
        private PIPointGridFormat _pipointgridformat = null;
        private string _destinationServer;
        private bool _isUpdateButtonEnabled = true;
        private bool _isPushButtonEnabled = false;
        private bool _isRefreshButtonEnabled = false;
        #endregion

        #region Properties
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
        public bool OptionCreateOnly { get; set; } = true;
        public bool OptionUpdateOnly { get; set; }
        public bool OptionCreateOrUpdate { get; set; }
        public bool IsUpdateButtonEnabled
        {
            get => _isUpdateButtonEnabled;
            set
            {
                SetProperty(ref _isUpdateButtonEnabled, value);
                OnPropertyChanged(nameof(IsUpdateButtonEnabled));
            }
        }
        public bool IsPushButtonEnabled
        {
            get => _isPushButtonEnabled;
            set
            {
                SetProperty(ref _isPushButtonEnabled, value);
                OnPropertyChanged(nameof(IsPushButtonEnabled));
            }
        }
        public bool IsRefreshButtonEnabled
        {
            get => _isRefreshButtonEnabled;
            set
            {
                SetProperty(ref _isRefreshButtonEnabled, value);
                OnPropertyChanged(nameof(IsRefreshButtonEnabled));
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
                o => UpdateTagsAttributes(),
                o => CanUpdateTagConfiguration());

            _buttonPushTags = new RelayCommand(
                o => PushTagsAttributes());

            _buttonRefresh = new RelayCommand(
                o => UpdateRowsUsingCurrentValues());
        }
        #endregion

        #region Methods
        public void UpdateTagsAttributes()
        {
            Logger.Info("Call method PushTagsConfigurationViewModel.UpdateTagsAttributes");
            try
            {
                // Get a tab with the current status of selected tags in the datagrid
                ReplicationManager.DataGridCollection.SelectedValues_FullTagsTabSize = ReplicationManager.DataGridCollection.GetSelectedValues();
                bool[] SelectedColumnStatus = ReplicationManager.DataGridCollection.SelectedValues_FullTagsTabSize;

                ReplicationManager.PIAttributesUpdateManager.UpdateTagsAttributes(
                    ReplicationManager.PIConnectionManager.PISourceServer,
                    ReplicationManager.PIConnectionManager.PITargetServer,
                    SelectedColumnStatus);

                PIReplicationManager.ReplicationManager.DataGridCollection.UpdateGrid();
                    OnPropertyChanged(nameof(Attributes));

                // Button IsPushButtonEnabled is now enabled for step 2
                IsPushButtonEnabled = true;
            }
            catch (Exception exc)
            {
                string p_ErrorMsg = $"Error with method PushTagsConfigurationViewModel.UpdateTagsAttributes. {exc.Message + "," + exc.StackTrace}";
                MessageBox.Show(p_ErrorMsg);
                Logger.Error(p_ErrorMsg);
            }            

            Logger.Info("End method PushTagsConfigurationViewModel.UpdateTagsAttributes");
        }
        public void PushTagsAttributes()
        {
            Logger.Info("Call method PushTagsConfigurationViewModel.PushTagsAttributes");
            FilesManager.CreateTagsOutputFile(ReplicationManager.PIAttributesUpdateManager.AttributesTagsList, BackupType.TargetServerBackup);
            
            // Get a tab with the current status of selected tags in the datagrid
            bool[] SelectedColumnStatus = ReplicationManager.DataGridCollection.SelectedValues_FullTagsTabSize;

            if (OptionCreateOnly)
            {
                PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.CreateAndPushTags(
                    PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer,
                    SelectedColumnStatus);
            }
            else if (OptionUpdateOnly)
            {
                PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.UpdateAndPushTags(
                    PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer,
                    SelectedColumnStatus);
                    
            }
            else if (OptionCreateOrUpdate)
            {
                PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.CreateOrUpdateAndPushTags(
                    PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer,
                    SelectedColumnStatus);
            }

            // Refresh button is available for step 3 
            IsRefreshButtonEnabled = true;

            // Update & create buttons locked
            IsUpdateButtonEnabled = false;
            IsPushButtonEnabled = false;

            UpdateRowsUsingCurrentValues();
            Logger.Info("End method PushTagsConfigurationViewModel.PushTagsAttributes");
        }
        public void UpdateRowsUsingCurrentValues()
        {
            // Get a tab with the current status of selected tags in the datagrid
            bool[] SelectedColumnStatus = ReplicationManager.DataGridCollection.SelectedValues_FullTagsTabSize;

            PIReplicationManager.ReplicationManager.DataGridCollection.CollectionTags.Clear();
            var v_tagList = PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.AttributesTagsList;

            for (int i = 0; i < SelectedColumnStatus.Length; i++)
            {
                // Display the row with current value and color status if selected
                ReplicationManager.PIAttributesUpdateManager.GetCurrentValues(PIReplicationManager.ReplicationManager.PIConnectionManager.PITargetServer, v_tagList[i], SelectedColumnStatus[i]);
                OnPropertyChanged(nameof(Attributes));
            }
        }
        private bool CanUpdateTagConfiguration()
        {
            return !string.IsNullOrEmpty(ReplicationManager.PIConnectionManager.PISourceServerName)
                    && !string.IsNullOrEmpty(ReplicationManager.PIConnectionManager.PITargetServerName)
                    && ReplicationManager.PIAttributesUpdateManager.AttributesTagsList.Count > 0;

        }
        #endregion
    }
}
