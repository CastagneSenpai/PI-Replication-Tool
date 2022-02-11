using Commands;
using Models;
using OSIsoft.AF.PI;
using System.Threading.Tasks;

namespace ViewModels
{
    internal class ConnectionViewModel : BaseViewModel, IPageViewModel
    {
        private string _selectedSourceServer;
        private string _selectedTargetServer;
        private readonly AsyncCommand _buttonConnectSourceServer;
        private readonly AsyncCommand _buttonConnectTargetServer;

        public PIReplicationManager PIReplicationManager = new PIReplicationManager();
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
        
        //public RelayCommand ButtonConnectSourceServer { get; set; }
        //public RelayCommand ButtonConnectTargetServer { get; set; }
        //public RelayCommand ButtonNextView { get; set; }        

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
