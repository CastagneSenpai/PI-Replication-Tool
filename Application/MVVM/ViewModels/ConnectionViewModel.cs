using Commands;
using Models;
using OSIsoft.AF.PI;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ConnectionViewModel : BaseViewModel, IPageViewModel
    {
        public PIReplicationManager PIReplicationManager = PIReplicationManager.ReplicationManager;

        #region PrivateAttributes
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
        #region BindingAttributes
        public string SelectedSourceServer
        {
            set
            {
                SetProperty(ref _selectedSourceServer, value);
                OnPropertyChanged(nameof(SelectedSourceServer));
                _buttonConnectSourceServer.RaiseCanExecuteChanged();
            }
            get => _selectedSourceServer;
        }
        public string SelectedTargetServer
        {
            set
            {
                SetProperty(ref _selectedTargetServer, value);
                OnPropertyChanged(nameof(SelectedTargetServer));
                _buttonConnectTargetServer.RaiseCanExecuteChanged();
            }
            get => _selectedTargetServer;
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

        // CONSTRUCTEUR
        public ConnectionViewModel()
        {
            var PILocalServers = PIServers.GetPIServers();
            ListSourceServer = PILocalServers;
            ListTargetServer = PILocalServers;

            _buttonConnectSourceServer = new AsyncCommand(ConnectPISourceServerAsync, CanConnectOnSourceServer);
            _buttonConnectTargetServer = new AsyncCommand(ConnectPITargetServerAsync, CanConnectOnTargetServer);
        }

        private async Task ConnectPISourceServerAsync()
        {
            
            SourceKOStatus = "Hidden";
            SourceOKStatus = "Hidden";
            SourceSpinnerStatus = "Visible";

            bool status = await PIReplicationManager.PIConnectionManager.ConnectToPISourceServerAsync(SelectedSourceServer);
            SourceSpinnerStatus = "Hidden";

            if (status)
                SourceOKStatus = "Visible";
            else
                SourceKOStatus = "Visible";
        }

        private bool CanConnectOnSourceServer()
        {
            return !string.IsNullOrEmpty(SelectedSourceServer);
        }

        private async Task ConnectPITargetServerAsync()
        {
            TargetKOStatus = "Hidden";
            TargetOKStatus = "Hidden";
            TargetSpinnerStatus = "Visible";

            bool status = await PIReplicationManager.PIConnectionManager.ConnectToPITargetServerAsync(SelectedTargetServer);
            TargetSpinnerStatus = "Hidden";

            if (status)
                TargetOKStatus = "Visible";
            else
                TargetKOStatus = "Visible";
        }

        private bool CanConnectOnTargetServer()
        {
            return !string.IsNullOrEmpty(SelectedTargetServer);
        }
    }
}
