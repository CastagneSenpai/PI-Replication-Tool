﻿using NLog;

namespace Models
{
    public sealed class PIReplicationManager
    {
        private static PIReplicationManager _replicationManager = null;
        private static readonly object padlock = new object();

        public static PIReplicationManager ReplicationManager
        {
            get
            {
                lock (padlock)
                {
                    if (_replicationManager == null)
                    {
                        _replicationManager = new PIReplicationManager();
                    }
                    return _replicationManager;
                }
            }
        }

        public PIConnectionManager PIConnectionManager = new PIConnectionManager();
        public PIAttributesUpdateManager PIAttributesUpdateManager = new PIAttributesUpdateManager();
        public PISiteBaseManager PISiteBaseManager = new PISiteBaseManager();
        public DataGridCollection DataGridCollection = new DataGridCollection();

        //public static readonly Logger Logger = LogManager.GetLogger("PIReplicationToolLogger");
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    }
}
