using Core;
using Models;
using OSIsoft.AF.PI;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModels
{
    internal class ConnectionViewModel : BaseViewModel, IPageViewModel
    {
        public PIReplicationManager PIReplicationManager = new PIReplicationManager();        
        public PIServers ListSourceServer { get; set; }
        public PIServers ListTargetServer { get; set; }
        public string SelectedSourceServer { get; set; }
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

            ButtonConnectSourceServer = new AsyncCommand(ConnectPISourceServerAsync);
            ButtonConnectTargetServer = new AsyncCommand(ConnectPITargetServerAsync);
            //ButtonNextView = new AsyncCommand();

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
            if (SelectedSourceServer.Length > 0)
                return true;
            else
                return false;
            //return SelectedSourceServer.Length > 0;
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
