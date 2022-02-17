using Commands;
using Models;

namespace ViewModels
{
    public class LoadTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        public PIReplicationManager replicationManager = PIReplicationManager.ReplicationManager;

        private string _sourceServer;
        public string SourceServer
        {
            get
            {
                return _sourceServer;
            }
            set
            {
                _sourceServer = value;
                OnPropertyChanged(nameof(SourceServer));
            }
        }

        private readonly RelayCommand _buttonLoadTags;
        public RelayCommand ButtonLoadTags => _buttonLoadTags;

        public LoadTagsConfigurationViewModel()
        {
            SourceServer = replicationManager.PIConnectionManager.PISourceServerName;
            //_buttonLoadTags = new RelayCommand(
            //    o => PIReplicationManager.PIConnectionManager.ConnectToPISourceServer(SelectedSourceServer));
        }

    }
}
