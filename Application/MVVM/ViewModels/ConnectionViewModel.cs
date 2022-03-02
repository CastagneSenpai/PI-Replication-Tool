using Commands;
using Models;
using OSIsoft.AF.PI;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ConnectionViewModel : BaseViewModel, IPageViewModel
    {
        private string _selectedSourceServer;
        private string _selectedTargetServer;
        private readonly AsyncCommand _buttonConnectSourceServer;
        private readonly AsyncCommand _buttonConnectTargetServer;

        public PIReplicationManager PIReplicationManager = PIReplicationManager.ReplicationManager;

        public PIServers ListSourceServer { get; set; }
        public PIServers ListTargetServer { get; set; }
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

        public IAsyncCommand ButtonConnectSourceServer => _buttonConnectSourceServer;
        public IAsyncCommand ButtonConnectTargetServer => _buttonConnectTargetServer;

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
    }
}
