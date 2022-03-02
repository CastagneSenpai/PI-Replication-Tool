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
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;

        private readonly CollectionViewSource _collectionViewSource = new CollectionViewSource();
        private readonly ObservableCollection<PIPointGridFormat> _collectionTags = new ObservableCollection<PIPointGridFormat>();

        public ICollectionView Attributes
        {
            get
            {
                if (_collectionViewSource.View != null)
                {
                    _collectionViewSource.View.CurrentChanged += (sender, e) => PIPointGridFormat = _collectionViewSource.View.CurrentItem as PIPointGridFormat;
                    return _collectionViewSource?.View;
                }
                return null;
            }
        }

        private PIPointGridFormat _pipointgridformat = null;
        public PIPointGridFormat PIPointGridFormat
        {
            get => this._pipointgridformat;
            set
            {
                this._pipointgridformat = value;
                OnPropertyChanged(nameof(PIPointGridFormat));
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
                _sourceServer = ReplicationManager.PIConnectionManager.PISourceServerName;
                OnPropertyChanged(nameof(SourceServer));
            }
        }

        private readonly RelayCommand _buttonLoadTags;
        public RelayCommand ButtonLoadTags => _buttonLoadTags;

        public LoadTagsConfigurationViewModel()
        {
            SourceServer = ReplicationManager.PIConnectionManager.PISourceServerName;
            _buttonLoadTags = new RelayCommand(
                o => LoadAttributes(),
                o => true);

            //sinon :
        }

        void LoadAttributes()
        {
            List<string> v_TagsNameList = new List<string>();
            FilesManager.ParseInputFileToTagsList(ref v_TagsNameList);

            ReplicationManager.PIAttributesUpdateManager.Clear();
            _collectionTags.Clear();

            ReplicationManager.PIAttributesUpdateManager.LoadTagsAttributes(ReplicationManager.PIConnectionManager.PISourceServer, v_TagsNameList);

            Populate();
            OnPropertyChanged("Attributes");

            FilesManager.CreateTagsOutputFile(ReplicationManager.PIAttributesUpdateManager.AttributesTagsList);
        }

        private void Populate()
        {
            foreach (var pipoint in ReplicationManager.PIAttributesUpdateManager.AttributesTagsList)
            {
                _collectionTags.Add(new PIPointGridFormat(
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
