using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModels
{
    internal class LoadTagsConfigurationViewModel : BaseViewModel
    {
        private ICommand _goToPushTagsConfigurationView;

        public ICommand GoToPushTagsConfigurationView
        {
            get
            {
                return _goToPushTagsConfigurationView ?? (_goToPushTagsConfigurationView = new RelayCommand(x =>
                {
                    Mediator.Notify("GoTo2Screen", "");
                }));
            }
        }
    }
}
