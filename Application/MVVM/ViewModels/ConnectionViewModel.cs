using Commands;
using Models;
using OSIsoft.AF.PI;
using System.Threading.Tasks;

namespace ViewModels
{
    internal class ConnectionViewModel : BaseViewModel, IPageViewModel
    {
        private string _selectedSourceServer;

        public PIReplicationManager PIReplicationManager = new PIReplicationManager();
        public PIServers ListSourceServer { get; set; }
        public PIServers ListTargetServer { get; set; }
        public string SelectedSourceServer
        {
            set
            {
                _selectedSourceServer = value;
                OnPropertyChanged(nameof(SelectedSourceServer));
            }
            get => _selectedSourceServer;
        }
        public string SelectedTargetServer { get; set; }
        //public RelayCommand ButtonConnectSourceServer { get; set; }
        //public RelayCommand ButtonConnectTargetServer { get; set; }
        //public RelayCommand ButtonNextView { get; set; }        
        public IAsyncCommand ButtonConnectSourceServer { get; set; }
        public IAsyncCommand ButtonConnectTargetServer { get; set; }

        // CONSTRUCTEUR
        public ConnectionViewModel()
        {
            var PILocalServers = PIServers.GetPIServers();
            ListSourceServer = PILocalServers;
            ListTargetServer = PILocalServers;

            ButtonConnectSourceServer = new AsyncCommand(ConnectPISourceServerAsync, CanConnectOnSourceServer);
            ButtonConnectTargetServer = new AsyncCommand(ConnectPITargetServerAsync);

            //ButtonConnectSourceServer = new RelayCommand(
            //    o => PIReplicationManager.PIConnectionManager.ConnectToPISourceServer(SelectedSourceServer));
            ////o => SelectedSourceServer.Length > 0);

            //ButtonConnectTargetServer = new RelayCommand(
            //    o => PIReplicationManager.PIConnectionManager.ConnectToPITargetServer(SelectedTargetServer));
            //o => SelectedTargetServer.Length > 0);

            // TODO : change function called to execute
            //ButtonNextView = new RelayCommand(
            //    o => PIReplicationManager.PIConnectionManager.ConnectToPITargetServer(SelectedTargetServer));
        }

        private async Task ConnectPISourceServerAsync()
        {
            await PIReplicationManager.PIConnectionManager.ConnectToPISourceServerAsync(SelectedSourceServer);
        }

        private bool CanConnectOnSourceServer()
        {
            //if (SelectedSourceServer?.Length > 0)
            //    return true;
            //else
            //    return false;
            //return SelectedSourceServer?.Length > 0;
            //if (SelectedSourceServer == null)
            //    return false;
            //else if (SelectedSourceServer.Length == 0)
            //    return false;
            //else
            //    return true;
            return !string.IsNullOrEmpty(SelectedSourceServer);
        }

        private async Task ConnectPITargetServerAsync()
        {
            await PIReplicationManager.PIConnectionManager.ConnectToPISourceServerAsync(SelectedTargetServer);
        }

        private bool CanConnectOnTargerServer()
        {
            if (SelectedTargetServer.Length > 0)
                return true;
            else
                return false;
            //return SelectedTargetServer.Length > 0;
        }
    }
}
