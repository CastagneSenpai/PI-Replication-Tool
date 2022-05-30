using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using System;

namespace Core
{
    public class NlogMemoryTarget : Target
    {
        public event EventHandler<string> OnLog;

        public NlogMemoryTarget(string name, LogLevel level) : this(name, level, level) { }
        public NlogMemoryTarget(string name, LogLevel minLevel, LogLevel maxLevel)
        {
            // important: we want LogManager.Configuration property assign behaviors \ magic to occur
            //   see: https://stackoverflow.com/a/3603571/1366179
            var config = LogManager.Configuration;

            // Add Target and Rule to their respective collections
            config.AddTarget(name, this);
            config.LoggingRules.Add(new LoggingRule("*", minLevel, maxLevel, this));

            LogManager.Configuration = config;
        }

        [Obsolete]
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            foreach (var logEvent in logEvents)
            {
                Write(logEvent.LogEvent);
            }
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(logEvent.LogEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            OnLog(this, logEvent.FormattedMessage);
        }

        // consider overriding WriteAsyncThreadSafe methods as well.
    }
}
