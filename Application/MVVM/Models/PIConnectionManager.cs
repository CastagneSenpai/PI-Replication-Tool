using NLog;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Models
{
    public class PIConnectionManager
    {
        // -------------------
        // PROPERTIES OF CLASS
        // -------------------
        private PIServers _localPIServerList = PIServers.GetPIServers();

        public PIServers LocalPIServersList
        {
            get { return _localPIServerList; }
            set { _localPIServerList = value; }
        }

        private List<PIServer> _connectedPIServersList = new List<PIServer>();

        public List<PIServer> ConnectedPIServersList
        {
            get { return _connectedPIServersList; }
            set { _connectedPIServersList = value; }
        }

        private string _piSourceServerName;

        public string PISourceServerName
        {
            get { return _piSourceServerName; }
            set { _piSourceServerName = value; }
        }

        private string _piTargetServerName;

        public string PITargetServerName
        {
            get { return _piTargetServerName; }
            set { _piTargetServerName = value; }
        }

        Logger Logger = NLog.LogManager.GetLogger("PIReplicationToolLogger");

        // -------------------
        // CONSTRUCTORS
        // -------------------
        public PIConnectionManager()
        {

        }

        // -------------------
        // FUNCTIONS
        // -------------------
         public async Task ConnectToPIServerAsync(string p_PIServerName)
        {           
            await Task.Run(() =>
            {
                PIServer PIServer;
                try
                {
                    Logger.Info("Trying to connect to " + p_PIServerName);
                    PIServer = PIServer.FindPIServer(p_PIServerName);
                    if (!PIServer.ConnectionInfo.IsConnected)
                    {
                        PIServer.Connect();
                        _connectedPIServersList.Add(PIServer);
                        Logger.Info("Successfully connected to " + p_PIServerName);
                        MessageBox.Show("Successfully connected to " + p_PIServerName);
                    }
                    else
                    {
                        Logger.Info("Already connected to " + p_PIServerName);
                        MessageBox.Show("Already connected to " + p_PIServerName);
                    }
                }
                catch (Exception)
                {
                    // Write log error
                    Logger.Error("Cannot connect to " + p_PIServerName);
                    MessageBox.Show("Cannot connect to " + p_PIServerName);
                    //throw e;
                }
            });
        }

        public async Task ConnectToPISourceServerAsync(string p_PIServerName)
        {
            this.PISourceServerName = p_PIServerName;
            await this.ConnectToPIServerAsync(p_PIServerName);
        }

        public async Task ConnectToPITargetServerAsync(string p_PIServerName)
        {
            this.PITargetServerName = p_PIServerName;
            await this.ConnectToPIServerAsync(p_PIServerName);
        }

        public void RefreshAllConnections()
        {
            foreach(var PIServ in _connectedPIServersList)
            {
                if(!PIServ.ConnectionInfo.IsConnected)
                {
                    try
                    {
                        Logger.Debug("Trying to reconnect to the server " + PIServ.Name);
                        PIServ.Connect();
                        Logger.Debug("Sucessfully reconnected to " + PIServ.Name);
                    }
                    catch (Exception e)
                    {
                        // Log the reconnection error and inform server has been removed
                        Logger.Error(e.Message);

                        // Remove server from connectedServerList and go next server
                        _connectedPIServersList.Remove(PIServ);
                        continue;
                    }
                }
            }
        }
    }
}
