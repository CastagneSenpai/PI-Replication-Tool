using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class NlogMemoryTarget : Target
    {
        public Action<string> Log = delegate { };

        public NlogMemoryTarget(string name, LogLevel level)
        {
            LogManager.Configuration.AddTarget(name, this);

            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", level, this));//This will ensure that existing rules are not overwritten
            LogManager.Configuration.Reload(); //This is important statement to reload all applied settings

            //SimpleConfigurator.ConfigureForTargetLogging (this, level); //use this if you are intending to use only NlogMemoryTarget  rule
        }
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(logEvent.LogEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            Log(logEvent.FormattedMessage);
        }

        [Obsolete]
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            foreach (var logEvent in logEvents)
            {
                Write(logEvent);
            }
        }
    }
}
