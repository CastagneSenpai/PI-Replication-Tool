﻿using OSIsoft.AF.PI;
using PI_Replication_Tool.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PI_Replication_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public ObservableCollection 

        public MainWindow()
        {
            InitializeComponent();

            //List<string> list = new List<string>();
            //list.Add("ezkdos"); list.Add("561616"); list.Add("cksdc64");

            //list<string> piservernames = new list<string>();
            //foreach (var server in piservers.getpiservers())
            //{
            //    piservernames.add(server.name);
            //}

            var PILocalServers = PIServers.GetPIServers();

            // Chargement de la liste des serveurs sources
            listSourceServer.ItemsSource = PILocalServers;

            // Chargement de la liste des serveurs cibles
            listTargetServer.ItemsSource = PILocalServers;
        }

        private void Button_Continue_Click(object sender, RoutedEventArgs e)
        {
            string sourceServer = listSourceServer.SelectedItem.ToString();
            string targetServer = listTargetServer.SelectedItem.ToString();

            PIConnectionManager PIConnection = new PIConnectionManager();
            
        }
        //var c = pIConnection.ConnectToServer("PI-CENTER-HQ");
        //var v = pIConnection.GetPIPointValue(c, "sinusoid");
        //MessageBox.Show(v.ToString());
    
    }
}
