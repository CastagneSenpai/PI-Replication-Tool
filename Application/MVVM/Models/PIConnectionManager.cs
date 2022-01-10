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
        private PIServers _piServers;        

        public PIServers PIServers
        {
            get { return PIServers.GetPIServers(); }
            set { _piServers = value; }
        }

        public PIConnectionManager()
        {
            //_piServers = new PIServers();
            //_piServers.GetPI
        }

        public PIServer ConnectToServer(string servername)
        {
            PIServer server = null;
            try
            {
                server = PIServer.FindPIServer(servername);
                server.Connect();

            }
            catch (Exception)
            {
                throw;
            }
            return server;
        }

        public AFValue GetPIPointValue(PIServer server, string tag)
        {
            var point = PIPoint.FindPIPoint(server, tag);
            var value = point.CurrentValue();

            return value;
        }
    }
}
