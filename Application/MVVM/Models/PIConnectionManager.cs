using NLog;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Models
{
    public class PIConnectionManager
    {
        // -------------------
        // PROPERTIES OF CLASS
        // -------------------

        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // Servers lists
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

        // Non-list servers
        private string _piSourceServerName;

        public string PISourceServerName
        {
            get { return _piSourceServerName; }
            set { _piSourceServerName = value; }
        }

        public PIServer PISourceServer
        {
            get { return PIServer.FindPIServer(_piSourceServerName); }
        }

        private string _piTargetServerName;

        public string PITargetServerName
        {
            get { return _piTargetServerName; }
            set { _piTargetServerName = value; }
        }

        public PIServer PITargetServer
        {
            get { return PIServer.FindPIServer(_piTargetServerName); }
        }


        // -------------------
        // CONSTRUCTORS
        // -------------------
        public PIConnectionManager()
        {

        }

        // -------------------
        // FUNCTIONS
        // -------------------
         public async Task<bool> ConnectToPIServerAsync(string p_PIServerName)
        {
            Logger.Info($"Call method PIConnectionManager.ConnectToPIServerAsync for {p_PIServerName}.");
            bool v_IsSuccess = false;
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
                        v_IsSuccess = true;
                    }
                    else
                    {
                        Logger.Info("Already connected to " + p_PIServerName);
                        v_IsSuccess = true;
                    }
                }
                catch (Exception)
                {
                    Logger.Error("Cannot connect to " + p_PIServerName);
                    v_IsSuccess = false;
                }
            });
            Logger.Info($"End method PIConnectionManager.ConnectToPIServerAsync for {p_PIServerName}.");
            return v_IsSuccess;
        }

        public async Task<bool> ConnectToPISourceServerAsync(string p_PIServerName)
        {
            this.PISourceServerName = p_PIServerName;
            return await this.ConnectToPIServerAsync(p_PIServerName);
        }

        public async Task<bool> ConnectToPITargetServerAsync(string p_PIServerName)
        {
            this.PITargetServerName = p_PIServerName;
            return await this.ConnectToPIServerAsync(p_PIServerName);
        }

        public void RefreshAllConnections()
        {
            Logger.Info($"Call method PIConnectionManager.RefreshAllConnections.");
            foreach (var PIServ in _connectedPIServersList)
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
            Logger.Info($"End method PIConnectionManager.RefreshAllConnections.");
        }
    }
}
