using OSIsoft.AF.PI;
using PI_Replication_Tool.Core;
using System.Collections.ObjectModel;
using System.Windows;

namespace PI_Replication_Tool.MVVM.ViewModels
{
    internal class MainViewModel
    {
        //public PIServers ListSourceServer { get; set; }
        //public PIServers ListTargetServer { get; set; }

        public ObservableCollection<PIServer> ListSourceServer;
        public ObservableCollection<PIServer> ListTargetServer;

        public RelayCommand ContinueButton { get; set; }

        public MainViewModel()
        {
            var PILocalServers = PIServers.GetPIServers();

            // Chargement de la liste des serveurs sources
            // ListSourceServer = PILocalServers;
            ListSourceServer = new ObservableCollection<PIServer>();
            ListTargetServer = new ObservableCollection<PIServer>();
            foreach (var PIServ in PILocalServers)
            {
                ListSourceServer.Add(PIServ);
                ListTargetServer.Add(PIServ);
            }

            // Chargement de la liste des serveurs cibles
            //ListTargetServer = PILocalServers;

        }
        private void Button_Continue_Click(object sender, RoutedEventArgs e)
        {
            //string SourceServer = ListSourceServer.SelectedItem.ToString();
            //string TargetServer = ListTargetServer.SelectedItem.ToString();
        }
    }
}
