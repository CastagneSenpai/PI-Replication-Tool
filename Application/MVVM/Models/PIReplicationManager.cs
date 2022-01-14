using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    internal class PIReplicationManager
    {
        public PIConnectionManager PIConnectionManager = new PIConnectionManager();
        public PIAttributesUpdateManager PIAttributesUpdateManager = new PIAttributesUpdateManager();
    }
}
