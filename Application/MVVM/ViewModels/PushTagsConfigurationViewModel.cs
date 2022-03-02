using Models;

namespace ViewModels
{
    internal class PushTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;
        public PushTagsConfigurationViewModel()
        {

        }
    }
}
