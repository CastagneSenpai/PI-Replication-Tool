using Core;
using System.Windows.Input;

namespace ViewModels
{
    internal class LoadTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        private ICommand _goToPushTagsConfigurationView;

        public ICommand GoToPushTagsConfigurationView
        {
            get
            {
                return _goToPushTagsConfigurationView ?? (_goToPushTagsConfigurationView = new RelayCommand(x =>
                {
                    Mediator.Notify("GoToLoadTagConfigurationScreen", "");
                }));
            }
        }

        public LoadTagsConfigurationViewModel()
        {

        }
    }
}
