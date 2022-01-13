using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OSIsoft.AF.PI;

namespace ViewModels
{
    internal class MainViewModel
    {
        //public PIServers ListSourceServer { get; set; }
        //public PIServers ListTargetServer { get; set; }

        public ObservableCollection<PIServer> ListSourceServer;
        public ObservableCollection<PIServer> ListTargetServer;
        public 

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
