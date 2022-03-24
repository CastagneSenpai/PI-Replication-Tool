using Commands;
using Models;
using OSIsoft.AF.PI;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ConnectionViewModel : BaseViewModel, IPageViewModel
    {
        #region Fields
        public PIReplicationManager PIReplicationManager = PIReplicationManager.ReplicationManager;

        private string _selectedSourceServer;
        private string _selectedTargetServer;
        #endregion

        #region Properties
        public PIServers ListSourceServer { get; set; }
        public PIServers ListTargetServer { get; set; }
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
        #endregion

        #region Commands
        private readonly AsyncCommand _buttonConnectSourceServer;
        private readonly AsyncCommand _buttonConnectTargetServer;

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
            await PIReplicationManager.PIConnectionManager.ConnectToPISourceServerAsync(SelectedSourceServer);
        }

        private bool CanConnectOnSourceServer()
        {
            return !string.IsNullOrEmpty(SelectedSourceServer);
        }

        private async Task ConnectPITargetServerAsync()
        {
            await PIReplicationManager.PIConnectionManager.ConnectToPITargetServerAsync(SelectedTargetServer);
        }

        private bool CanConnectOnTargetServer()
        {
            return !string.IsNullOrEmpty(SelectedTargetServer);
        }
        #endregion
    }
}
