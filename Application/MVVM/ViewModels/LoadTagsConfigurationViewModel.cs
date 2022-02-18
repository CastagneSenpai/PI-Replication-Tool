using Commands;
using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace ViewModels
{
    public class LoadTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        private readonly PIReplicationManager _replicationManager = PIReplicationManager.ReplicationManager;

        private readonly CollectionViewSource cvs = new CollectionViewSource();
        private ObservableCollection<PIPoint> col = new ObservableCollection<PIPoint>();

        List<IDictionary<string, object>> dicti = new List<IDictionary<string, object>>();

        public ICollectionView Attributes
        {
            get
            {
                if (cvs.Source == null)
                {
                    LoadAttributes();
                    Populate();
                    cvs.View.CurrentChanged += (sender, e) => PIPoint = cvs.View.CurrentItem as PIPoint;
                }
                return cvs.View;
            }
        }

        private PIPoint _pipoint = null;
        public PIPoint PIPoint
        {
            get => this._pipoint;
            set
            {
                this._pipoint = value;
                OnPropertyChanged(nameof(PIPoint));
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
            //List<IDictionary<string,object>> dicti = new List<IDictionary<string, object>>();           
            FilesManager.ParseInputFileToTagsList(ref liste);
            _replicationManager.PIAttributesUpdateManager.LoadAttributes(_replicationManager.PIConnectionManager.ConnectedPIServersList[0], liste, ref dicti);
            //Populate();
        }

        private void Populate()
        {
            foreach(var pipoint in dicti)
            {
                col.Add(new PIPoint(
                    pipoint["tag"] as string
                    ));
            }
            cvs.Source = col;
        }

    }
}
