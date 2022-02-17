using Commands;
using Core;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IPageViewModel
    {
        public PIReplicationManager PIReplicationManager = new PIReplicationManager();

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        private ICommand _buttonNextView;
        public ICommand ButtonNextView
        {
            get
            {
                return _buttonNextView ?? (_buttonNextView = new RelayCommand(x =>
                {
                    //Mediator.Notify("GoToLoadTagConfigurationScreen", "");
                    ChangeViewModel(PageViewModels[1]);
                }
                ));
            }
        }

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

            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        private void OnGoConnectionScreen(object obj)
        {
            ChangeViewModel(PageViewModels[0]);
        }

        private void OnGoLoadTagConfigurationScreen(object obj)
        {
            ChangeViewModel(PageViewModels[1]);
        }

        private void OnGoPushTagConfigurationScreen(object obj)
        {
            ChangeViewModel(PageViewModels[2]);
        }

        public MainWindowViewModel()
        {
            // Add available pages and set page
            PageViewModels.Add(new ConnectionViewModel());
            PageViewModels.Add(new LoadTagsConfigurationViewModel());
            PageViewModels.Add(new PushTagsConfigurationViewModel());

            CurrentPageViewModel = PageViewModels[0];

            Mediator.Subscribe("GoToConnectionScreen", OnGoConnectionScreen);
            Mediator.Subscribe("GoToLoadTagConfigurationScreen", OnGoLoadTagConfigurationScreen);
            Mediator.Subscribe("GoToPushTagConfigurationScreen", OnGoPushTagConfigurationScreen);
        }
    }
}