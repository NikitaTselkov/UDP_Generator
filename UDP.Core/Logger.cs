using System.Data;

namespace UDP.Core
{
    public enum LoggerTypes
    { 
        Message,
        Error,
        Info
    }

    public class LogEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public LoggerTypes Type { get; private set; }

        public LogEventArgs(string message, LoggerTypes type)
        {
            Message = message;
            Type = type;
        }
    }

    public class Logger
    {
        public delegate void LogChangedHandler(LogEventArgs e);
        public static event LogChangedHandler OnLogChanged;

        private string _title { get; init; }

        public Logger(string title)
        {
            _title = title;
        }

        public void PushMessage(string message, LoggerTypes type = LoggerTypes.Message)
        {
            message = _title + ": " + message;

            Console.WriteLine(type + " " + message);

            if (type == LoggerTypes.Message)
            {
                var args = new LogEventArgs(message, type);
                OnLogChanged?.Invoke(args);
            }
        }
    }
}
