using Commands;
using Models;
using OSIsoft.AF.Asset;
using OSIsoft.AF.PI;
using System.Collections.Generic;

namespace ViewModels
{
    public class LoadTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        private PIReplicationManager _replicationManager = PIReplicationManager.ReplicationManager;

        List<IDictionary<string, object>> dicti = new List<IDictionary<string, object>>();

        private List<IDictionary<string, object>> _attributes;
        public List<IDictionary<string, object>> Attributes
        {
            get { return _attributes; }
            set
            {
                _attributes = dicti;
                OnPropertyChanged(nameof(Attributes));
            }
        }

        private string _sourceServer;
        public string SourceServer
        {
            get
            {
                return _sourceServer;
            }
            set
            {
                _sourceServer = _replicationManager.PIConnectionManager.PISourceServerName;
                OnPropertyChanged(nameof(SourceServer));
            }
        }

        public List<string> uneListe = new List<string>();
        PIPoint unPoint = null;
        AFValue uneValeur = null;


        private readonly RelayCommand _buttonLoadTags;
        public RelayCommand ButtonLoadTags => _buttonLoadTags;

        public LoadTagsConfigurationViewModel()
        {
            SourceServer = _replicationManager.PIConnectionManager.PISourceServerName;
            _buttonLoadTags = new RelayCommand(
                o => LoadAttributes(),
                o => true);
        }

        void LoadAttributes()
        {
            List<string> liste = new List<string>();
            dicti.Clear();
            //List<IDictionary<string,object>> dicti = new List<IDictionary<string, object>>();           
            FilesManager.ParseInputFileToTagsList(ref liste);
            _replicationManager.PIAttributesUpdateManager.LoadAttributes(_replicationManager.PIConnectionManager.ConnectedPIServersList[0], liste, ref dicti);
            FilesManager.CreateTagsOutputFile(dicti);
        }
    }
}
