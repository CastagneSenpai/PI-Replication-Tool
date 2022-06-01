using Commands;
using Models;
using OSIsoft.AF.PI;
using System.Threading.Tasks;
using NLog;

namespace ViewModels
{
    public class ConnectionViewModel : BaseViewModel, IPageViewModel
    {
        

        #region Fields
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public PIReplicationManager PIReplicationManager = PIReplicationManager.ReplicationManager;

        private string _selectedSourceServer;
        private string _selectedTargetServer;
        private string _sourceKOStatus = "Hidden";
        private string _sourceOKStatus = "Hidden";
        private string _sourceSpinnerStatus = "Hidden";
        private string _targetKOStatus = "hidden";
        private string _targetOKStatus = "hidden";
        private string _targetSpinnerStatus = "Hidden";
        private readonly AsyncCommand _buttonConnectSourceServer;
        private readonly AsyncCommand _buttonConnectTargetServer;
        #endregion

        #region Properties
        public string SelectedSourceServer
        {
            get => _selectedSourceServer;
            set
            {
                SetProperty(ref _selectedSourceServer, value);
                OnPropertyChanged(nameof(SelectedSourceServer));
                _buttonConnectSourceServer.RaiseCanExecuteChanged();
            }
        }
        public string SelectedTargetServer
        {
            get => _selectedTargetServer;
            set
            {
                SetProperty(ref _selectedTargetServer, value);
                OnPropertyChanged(nameof(SelectedTargetServer));
                _buttonConnectTargetServer.RaiseCanExecuteChanged();
            }
        }
        public PIServers ListSourceServer { get; set; }
        public PIServers ListTargetServer { get; set; }
        public string SourceKOStatus
        {
            set
            {
                SetProperty(ref _sourceKOStatus, value);
                //OnPropertyChanged(nameof(SourceKOStatus));
            }
            get => _sourceKOStatus;
        }
        public string SourceOKStatus
        {
            set
            {
                SetProperty(ref _sourceOKStatus, value);
                //OnPropertyChanged(nameof(SourceOKStatus));
            }
            get => _sourceOKStatus;
        }
        public string SourceSpinnerStatus
        {
            set
            {
                SetProperty(ref _sourceSpinnerStatus, value);
                //OnPropertyChanged(nameof(SourceSpinnerStatus));
            }
            get => _sourceSpinnerStatus;
        }
        public string TargetKOStatus
        {
            set
            {
                SetProperty(ref _targetKOStatus, value);
                //OnPropertyChanged(nameof(SourceKOStatus));
            }
            get => _targetKOStatus;
        }
        public string TargetOKStatus
        {
            set
            {
                SetProperty(ref _targetOKStatus, value);
                //OnPropertyChanged(nameof(SourceOKStatus));
            }
            get => _targetOKStatus;
        }
        public string TargetSpinnerStatus
        {
            set
            {
                SetProperty(ref _targetSpinnerStatus, value);
                //OnPropertyChanged(nameof(SourceSpinnerStatus));
            }
            get => _targetSpinnerStatus;
        }
        public IAsyncCommand ButtonConnectSourceServer => _buttonConnectSourceServer;
        public IAsyncCommand ButtonConnectTargetServer => _buttonConnectTargetServer;
        #endregion

        #region Constructor
        public ConnectionViewModel()
        {
            var PILocalServers = PIServers.GetPIServers();
            ListSourceServer = PILocalServers;
            ListTargetServer = PILocalServers;

            _buttonConnectSourceServer = new AsyncCommand(ConnectPISourceServerAsync, CanConnectOnSourceServer);
            _buttonConnectTargetServer = new AsyncCommand(ConnectPITargetServerAsync, CanConnectOnTargetServer);
        }
        #endregion

        #region Methods
        private async Task ConnectPISourceServerAsync()
        {
            Logger.Info("Call method ConnectionViewModel.ConnectPISourceServerAsync");
            this.SourceKOStatus = "Hidden";
            this.SourceOKStatus = "Hidden";
            this.SourceSpinnerStatus = "Visible";

            bool status = await PIReplicationManager.PIConnectionManager.ConnectToPISourceServerAsync(SelectedSourceServer);
            this.SourceSpinnerStatus = "Hidden";

            if (status)
                this.SourceOKStatus = "Visible";
            else
                this.SourceKOStatus = "Visible";
            Logger.Info("End method ConnectionViewModel.ConnectPISourceServerAsync");
        }
        private bool CanConnectOnSourceServer()
        {
            return !string.IsNullOrEmpty(SelectedSourceServer);
        }
        private async Task ConnectPITargetServerAsync()
        {
            Logger.Info("Call method ConnectionViewModel.ConnectPITargetServerAsync");

            this.TargetKOStatus = "Hidden";
            this.TargetOKStatus = "Hidden";
            this.TargetSpinnerStatus = "Visible";

            bool status = await PIReplicationManager.PIConnectionManager.ConnectToPITargetServerAsync(SelectedTargetServer);
            this.TargetSpinnerStatus = "Hidden";

            if (status)
                this.TargetOKStatus = "Visible";
            else
                this.TargetKOStatus = "Visible";

            Logger.Info("End method ConnectionViewModel.ConnectPITargetServerAsync");
        }
        private bool CanConnectOnTargetServer()
        {
            return !string.IsNullOrEmpty(SelectedTargetServer);
        }
        #endregion
    }
}
