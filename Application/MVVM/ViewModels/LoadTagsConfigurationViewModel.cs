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

        private readonly CollectionViewSource _collectionViewSource = new CollectionViewSource();
        private readonly ObservableCollection<PIPoint> _collectionTags = new ObservableCollection<PIPoint>();

        // TODO : to delete and use AttributeTagsList in PIAttributesUpdateManager instead !
        // TO DELETE : List<IDictionary<string, object>> AttributesTagsList = new List<IDictionary<string, object>>();

        public ICollectionView Attributes
        {
            get
            {
                if (_collectionViewSource.Source == null)
                {
                    LoadAttributes();
                    Populate();
                    _collectionViewSource.View.CurrentChanged += (sender, e) => PIPoint = _collectionViewSource.View.CurrentItem as PIPoint;
                }
                return _collectionViewSource.View;
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
            // if bouton ratio option 1
            _buttonLoadTags = new RelayCommand(
                o => LoadAttributes(),
                o => true);

            //sinon :
        }

        void LoadAttributes()
        {
            List<string> v_TagsNameList = new List<string>();
            FilesManager.ParseInputFileToTagsList(ref v_TagsNameList);

            _replicationManager.PIAttributesUpdateManager.Clear();

            // TODO : Changer le serveur ConnectedPIServersList[0] >> SelectedSourceServer
            _replicationManager.PIAttributesUpdateManager.LoadTagsAttributes(_replicationManager.PIConnectionManager.PISourceServer, v_TagsNameList);
            
            // TODO : A réactiver lorsque chargement du tableau sur le bouton
            //FilesManager.CreateTagsOutputFile(AttributesTagsList);
        }

        private void Populate()
        {
            foreach (var pipoint in _replicationManager.PIAttributesUpdateManager.AttributesTagsList)
            {
                _collectionTags.Add(new PIPoint(
                    pipoint["tag"] as string,
                    pipoint["instrumenttag"] as string,
                    pipoint["pointtype"] as string,
                    pipoint["pointsource"] as string,
                    int.Parse(pipoint["location1"].ToString()),
                    float.Parse(pipoint["zero"].ToString()),
                    float.Parse(pipoint["typicalvalue"].ToString()),
                    float.Parse(pipoint["span"].ToString()),
                    int.Parse(pipoint["compressing"].ToString()),
                    float.Parse(pipoint["compdev"].ToString()),
                    float.Parse(pipoint["compdevpercent"].ToString()),
                    float.Parse(pipoint["compmin"].ToString()),
                    float.Parse(pipoint["excDev"].ToString()),
                    float.Parse(pipoint["excMin"].ToString()),
                    float.Parse(pipoint["excMax"].ToString()),
                    float.Parse(pipoint["excdevpercent"].ToString()),
                    float.Parse(pipoint["compdevpercent"].ToString()),
                    pipoint["datasecurity"].ToString(),
                    pipoint["ptsecurity"].ToString()
                    ));
            }
            _collectionViewSource.Source = _collectionTags;
        }

    }
}
