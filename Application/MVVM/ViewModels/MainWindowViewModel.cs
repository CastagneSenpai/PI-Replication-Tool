using Commands;
using Core;
using Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IPageViewModel
    {
        public PIReplicationManager PIReplicationManager = PIReplicationManager.ReplicationManager;
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private ICommand _buttonNextView;
        public ICommand ButtonNextView
        {
            get
            {
                return _buttonNextView ?? (_buttonNextView = new RelayCommand(x =>
                {
                    Mediator.Instance.Notify(GetNextViewModel(), "");
                },
               o =>
               {
                   if (CurrentPageViewModel.ToString().Equals("ViewModels.ConnectionViewModel"))
                   {
                       if (PIReplicationManager.PIConnectionManager.PISourceServer != null && PIReplicationManager.PIConnectionManager.PITargetServer != null)
                           return PIReplicationManager.PIConnectionManager.PISourceServer.ConnectionInfo.IsConnected && PIReplicationManager.PIConnectionManager.PITargetServer.ConnectionInfo.IsConnected;
                   }
                   else if (CurrentPageViewModel.ToString().Equals("ViewModels.LoadTagsConfigurationViewModel"))
                   {
                       if (PIReplicationManager.PIAttributesUpdateManager.AttributesTagsList.Count > 0)
                           return PIReplicationManager.PIAttributesUpdateManager.IsLoadingTimeOver;
                   }
                   return false;
               }
                ));
            }
        }
        private ICommand _connectionMenuButton;
        public ICommand ConnectionMenuButton
        {
            get
            {
                return _connectionMenuButton ?? (_connectionMenuButton = new RelayCommand(x =>
                {
                    Mediator.Instance.Notify("GoToConnectionScreen", "");
                }
                ));
            }
        }
        private ICommand _loadTagsAttributesMenuButton;
        public ICommand LoadTagsAttributesMenuButton
        {
            get
            {
                return _loadTagsAttributesMenuButton ?? (_loadTagsAttributesMenuButton = new RelayCommand(x =>
                {
                    Mediator.Instance.Notify("GoToLoadTagConfigurationScreen", "");
                }
                ));
            }
        }
        private ICommand _pushTagsAttributesMenuButton;
        public ICommand PushTagsAttributesMenuButton
        {
            get
            {
                return _pushTagsAttributesMenuButton ?? (_pushTagsAttributesMenuButton = new RelayCommand(x =>
                {
                    Mediator.Instance.Notify("GoToPushTagConfigurationScreen", "");
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
        private bool _isLoadingMenuEnabled;
        public bool IsLoadingMenuEnabled
        {
            get => _isLoadingMenuEnabled;
            set
            {
                SetProperty(ref _isLoadingMenuEnabled, value);
                OnPropertyChanged(nameof(IsLoadingMenuEnabled));
            }
        }
        private bool _isPushMenuEnabled;
        public bool IsPushMenuEnabled
        {
            get => _isPushMenuEnabled;
            set
            {
                SetProperty(ref _isPushMenuEnabled, value);
                OnPropertyChanged(nameof(IsPushMenuEnabled));
            }
        }
        private string _menuNameDisplayed;
        public string MenuNameDisplayed
        {
            get => _menuNameDisplayed;
            set
            {
                SetProperty(ref _menuNameDisplayed, value);
                OnPropertyChanged(nameof(MenuNameDisplayed));
            }
        }
        private Visibility _visibilityButtonNext;
        public Visibility VisibilityButtonNext
        {
            get => _visibilityButtonNext;
            set
            {
                SetProperty(ref _visibilityButtonNext, value);
                OnPropertyChanged(nameof(VisibilityButtonNext));
            }
        }
        private Visibility _visibilityButtonExit;
        public Visibility VisibilityButtonExit { get => _visibilityButtonExit; set => SetProperty(ref _visibilityButtonExit, value); }
        private RelayCommand buttonExit;
        public ICommand ButtonExit
        {
            get
            {
                if (buttonExit == null)
                {
                    buttonExit = new RelayCommand(PerformButtonExit);
                }

                return buttonExit;
            }
        }
        private void PerformButtonExit(object commandParameter)
        {
            Logger.Info("PI Replication Tool is closing.");
            Application.Current.Shutdown();
        }
        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }
        private string GetFirstViewModel()
        {
            return "GoToConnectionScreen";
        }
        private string GetNextViewModel()
        {
            var index = PageViewModels.FindIndex(vm => vm == CurrentPageViewModel);
            IPageViewModel v_nextViewModel = PageViewModels.Find(vm => vm == PageViewModels[index]);
            switch (v_nextViewModel.ToString())
            {
                case "ViewModels.ConnectionViewModel":
                    return "GoToLoadTagConfigurationScreen";
                case "ViewModels.LoadTagsConfigurationViewModel":
                    return "GoToPushTagConfigurationScreen";
                default:
                    // Go to main page in case of errors
                    return "GoToConnectionScreen";
            }
        }
        private void OnGoConnectionScreen(object obj)
        {
            ChangeViewModel(PageViewModels[0]);
            IsLoadingMenuEnabled = false;
            IsPushMenuEnabled = false;
            MenuNameDisplayed = "Connection to PI source and target servers for the replication";
            VisibilityButtonNext = Visibility.Visible;
            VisibilityButtonExit = Visibility.Hidden;
        }
        private void OnGoLoadTagConfigurationScreen(object obj)
        {
            ChangeViewModel(PageViewModels[1]);
            IsLoadingMenuEnabled = true;
            IsPushMenuEnabled = false;
            MenuNameDisplayed = "Load the tags from the PI source server using an input mode";
            VisibilityButtonNext = Visibility.Visible;
            VisibilityButtonExit = Visibility.Hidden;

        }
        private void OnGoPushTagConfigurationScreen(object obj)
        {
            ChangeViewModel(PageViewModels[2]);
            IsLoadingMenuEnabled = true;
            IsPushMenuEnabled = true;
            MenuNameDisplayed = "Update and push the tag configuration to the target PI Server";
            VisibilityButtonNext = Visibility.Hidden;
            VisibilityButtonExit = Visibility.Visible;
        }
        public void LogTextUpdate()
        {
            throw new NotImplementedException();
        }
        public MainWindowViewModel()
        {
            Logger.Info("PI Replication Tool is starting...");

            // Add available pages and set page
            PageViewModels.Add(new ConnectionViewModel());
            PageViewModels.Add(new LoadTagsConfigurationViewModel());
            PageViewModels.Add(new PushTagsConfigurationViewModel());

            //CurrentPageViewModel = PageViewModels[0];

            Mediator.Instance.Subscribe("GoToConnectionScreen", OnGoConnectionScreen);
            Mediator.Instance.Subscribe("GoToLoadTagConfigurationScreen", OnGoLoadTagConfigurationScreen);
            Mediator.Instance.Subscribe("GoToPushTagConfigurationScreen", OnGoPushTagConfigurationScreen);

            Mediator.Instance.Notify(GetFirstViewModel());

            Logger.Info("PI Replication Tool interface is loaded.");
        }


    }
}