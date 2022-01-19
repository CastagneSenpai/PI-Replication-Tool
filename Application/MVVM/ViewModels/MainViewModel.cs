﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OSIsoft.AF.PI;
using Core;
using Models;
using System.Web.UI.WebControls;
using System.Windows.Input;

namespace ViewModels
{
    internal class ConnectionViewModel
    {
        public PIReplicationManager PIReplicationManager = new PIReplicationManager();
        public PIServers ListSourceServer { get; set; }
        public PIServers ListTargetServer { get; set; }
        public string SelectedSourceServer { get; set; }
        public string SelectedTargetServer { get; set; }
        public RelayCommand ButtonConnectSourceServer { get; set; }
        public RelayCommand ButtonConnectTargetServer { get; set; }
        public RelayCommand ButtonContinueNextView { get; set; }
        

        public ConnectionViewModel()
        {
            var PILocalServers = PIServers.GetPIServers();
            ListSourceServer = PILocalServers;
            ListTargetServer = PILocalServers;

            // Action to connect to selected PI source server
            ButtonConnectSourceServer = new RelayCommand(
                o => PIReplicationManager.PIConnectionManager.ConnectToPISourceServer(SelectedSourceServer));
            //o => SelectedSourceServer.Length > 0);

            // Action to connect to selected PI target server
            ButtonConnectTargetServer = new RelayCommand(
                o => PIReplicationManager.PIConnectionManager.ConnectToPITargetServer(SelectedTargetServer));
            //o => SelectedTargetServer.Length > 0);

            // Action to go to the next view
            // TODO : command to use ButtonContinueNextView
            MessageBox.Show(SelectedSourceServer);
        }
    }
}
