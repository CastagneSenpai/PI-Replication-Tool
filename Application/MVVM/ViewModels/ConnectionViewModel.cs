using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OSIsoft.AF.PI;
using Core;
using Models;
using System.Web.UI.WebControls;
using System.Windows.Input;

namespace ViewModels
{
    internal class ConnectionViewModel : BaseViewModel
    {
        public PIReplicationManager PIReplicationManager = new PIReplicationManager();
        public PIServers ListSourceServer { get; set; }
        public PIServers ListTargetServer { get; set; }
        public string SelectedSourceServer { get; set; }
        public string SelectedTargetServer { get; set; }
        public RelayCommand ButtonConnectSourceServer { get; set; }
        public RelayCommand ButtonConnectTargetServer { get; set; }
        public RelayCommand ButtonNextView { get; set; }

        // NAVIGATION
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                _currentPageViewModel = value;
                OnPropertyChanged("CurrentPageViewModel");
            }
        }

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        private void OnGo1Screen(object obj)
        {
            ChangeViewModel(PageViewModels[0]);
        }


        
        
        // CONSTRUCTEUR
        public ConnectionViewModel()
        {
            // Add available pages and set page
            PageViewModels.Add(new LoadTagsConfigurationViewModel());
            PageViewModels.Add(new PushTagsConfigurationViewModel());

            CurrentPageViewModel = PageViewModels[0];

            //Mediator.Subscribe("GoTo1Screen", OnGo1Screen);
            //Mediator.Subscribe("GoTo2Screen", OnGo2Screen);


            var PILocalServers = PIServers.GetPIServers();
            ListSourceServer = PILocalServers;
            ListTargetServer = PILocalServers;

            ButtonConnectSourceServer = new RelayCommand(
                o => PIReplicationManager.PIConnectionManager.ConnectToPISourceServer(SelectedSourceServer));
            //o => SelectedSourceServer.Length > 0);

            ButtonConnectTargetServer = new RelayCommand(
                o => PIReplicationManager.PIConnectionManager.ConnectToPITargetServer(SelectedTargetServer));
            //o => SelectedTargetServer.Length > 0);

            // TODO : change function called to execute
            ButtonNextView = new RelayCommand(
                o => PIReplicationManager.PIConnectionManager.ConnectToPITargetServer(SelectedTargetServer));

            
        }
    }
}
