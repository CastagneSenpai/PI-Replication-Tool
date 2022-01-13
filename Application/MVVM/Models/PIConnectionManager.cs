using NLog;
using OSIsoft.AF.Asset;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Replication_Tool.MVVM.Models
{
    internal class PIConnectionManager
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

        private PIServers _connectedPIServersList = new PIServers();

        public PIServers ConnectedPIServersList
        {
            get { return _connectedPIServersList; }
            set { _connectedPIServersList = value; }
        }

        Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // -------------------
        // CONSTRUCTORS
        // -------------------
        public PIConnectionManager()
        {

        }

        // -------------------
        // FUNCTIONS
        // -------------------
        public PIServer ConnectToPIServer(string p_PIServerName)
        {
            PIServer PIServer = null;
            try
            {
                Logger.Info("Trying to connect to " + p_PIServerName);
                PIServer = PIServer.FindPIServer(p_PIServerName);
                PIServer.Connect();
                Logger.Info("Successfully connected to " + p_PIServerName);
            }
            catch (Exception e)
            {
                // Write log error
                Logger.Error("Cannot connect to " + p_PIServerName);
                throw e;
            }
            _connectedPIServersList.Add(PIServer.Name);
            return PIServer;
        }

        public void ConnectToPIServer(PIServer p_PIServer)
        {
            try
            {
                Logger.Info("Trying to connect to " + p_PIServer.Name);
                p_PIServer.Connect();
                Logger.Info("Successfully connected to " + p_PIServer.Name);
            }
            catch (Exception e)
            {
                // Write log error
                Logger.Error("Cannot connect to " + p_PIServer.Name);
                throw e;
            }
            _connectedPIServersList.Add(p_PIServer.Name);
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
                        _connectedPIServersList.Remove(PIServ.Name);
                        continue;
                    }
                }
            }
        }
    }
}
