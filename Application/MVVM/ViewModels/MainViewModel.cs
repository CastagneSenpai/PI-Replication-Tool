using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OSIsoft.AF.PI;
using Core;
using PI_Replication_Tool.MVVM.Models;
using System.Web.UI.WebControls;

namespace ViewModels
{
    internal class MainViewModel
    {
        //public PIServers ListSourceServer { get; set; }
        //public PIServers ListTargetServer { get; set; }
        public ObservableCollection<PIServer> ListSourceServer;
        public string SelectedSourceServer { get; set; }


        //public ObservableCollection<PIServer> ListTargetServer;
        public PIConnectionManager ConnectionManager = new PIConnectionManager();
        public RelayCommand ButtonConnectServer { get; set; }

        public MainViewModel()
        {
            // Action to connect to selected PI source server
            ButtonConnectServer = new RelayCommand(
                // Appel de la fonction de connexion au serveur
                o => ConnectionManager.ConnectToPIServer("AOEPTTA-APPIL01"),

                // Condition : serveur selectionné dans la liste
                o => true);
                // TODO: o => ListSourceServer.SelectedValue != null);


            var PILocalServers = PIServers.GetPIServers();


            // Chargement de la liste des serveurs sources
            // ListSourceServer = PILocalServers;
            //ListSourceServer = new ObservableCollection<PIServer>();
            //ListTargetServer = new ObservableCollection<PIServer>();
            //foreach (var PIServ in PILocalServers)
            //{
            //    ListSourceServer.Add(PIServ);
            //    ListTargetServer.Add(PIServ);
            //}


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
