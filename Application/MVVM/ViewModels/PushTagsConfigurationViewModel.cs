using Commands;
using Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace ViewModels
{
    public class PushTagsConfigurationViewModel : BaseViewModel, IPageViewModel
    {
        public PIReplicationManager ReplicationManager = PIReplicationManager.ReplicationManager;

        private readonly CollectionViewSource _collectionViewSource = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionViewSource;
        private readonly ObservableCollection<PIPointGridFormat> _collectionTags = PIReplicationManager.ReplicationManager.DataGridCollection.CollectionTags;

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

        private string _destinationServer;
        public string DestinationServer
        {
            get
            {
                return _destinationServer;
            }
            set
            {
                _destinationServer = ReplicationManager.PIConnectionManager.PITargetServerName;
                OnPropertyChanged(nameof(DestinationServer));
            }
        }

        private readonly RelayCommand _buttonUpdateTags;
        public RelayCommand ButtonUpdateTags => _buttonUpdateTags;

        //private readonly RelayCommand _buttonPushTags;
        //public RelayCommand ButtonPushTags => _buttonPushTags;

        public PushTagsConfigurationViewModel()
        {
            DestinationServer = ReplicationManager.PIConnectionManager.PITargetServerName;

            //_buttonUpdateTags = new RelayCommand(
            //    o => this.ReplicationManager.PIAttributesUpdateManager.UpdateTagsAttributes(
            //        ReplicationManager.PIConnectionManager.PISourceServer,
            //        ReplicationManager.PIConnectionManager.PITargetServer),
            //    o => true);

            _buttonUpdateTags = new RelayCommand(
                o => UpdateTagsAttributes(),
                o => true);
        }

        public void UpdateTagsAttributes()
        {
            ReplicationManager.PIAttributesUpdateManager.UpdateTagsAttributes(
                ReplicationManager.PIConnectionManager.PISourceServer,
                ReplicationManager.PIConnectionManager.PITargetServer);

            Populate();
            OnPropertyChanged("Attributes");
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


    }
}
