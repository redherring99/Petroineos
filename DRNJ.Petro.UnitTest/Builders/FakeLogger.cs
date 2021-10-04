using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRNJ.Petro.UnitTest.Builders
{
    public interface IFakeLogger<T> : IFakeLogger, ILogger<T>
    {

    }

    public interface IFakeLogger : ILogger, IDisposable
    {
        List<string> DebugLog { get; }

        /// <summary>
        /// Gets or sets the info log.
        /// </summary>
        List<string> InfoLog { get; }

        /// <summary>
        /// Gets or sets the error log.
        /// </summary>
        List<string> ErrorLog { get; }

        List<Exception> ExceptionLog { get; }

        List<LogMessage> LogMessages { get; }
    }


    public class LogMessage
    {
        public LogMessage() { }
        public LogMessage(
                 LogLevel logLevel,
                 EventId eventId,
                 Type type,
                 Exception exception,
                 string Message)
        {
            this.LogLevel = logLevel;
            this.EventId = eventId;
            this.Type = type;
            this.Exception = exception;
            this.Message = Message;
        }
        public LogLevel LogLevel { get; set; }
        public EventId EventId { get; set; }
        public Exception Exception { get; set; }

        public Type Type { get; set; }
        public string Message { get; set; }

    }




    /// <summary>
    /// Fake Logger as can't mock MS Extentions methods....
    /// So MS create .net Core, the create DI infrastructure, they create an ILogging infrastructure
    /// but you can't mock the logger with mocking framework (NSubstitute)
    /// https://github.com/nsubstitute/NSubstitute/issues/539
    ///
    /// "LogError is a non-virtual extension method, so we can not reliably mock this with NSubstitute.
    /// Depending on its implementation, we might be able to get it to work. Looking at the source, it calls
    /// logger.Log(LogLevel.Error, message, args), which is itself an extension method. Eventually,
    /// it gets down to this call."
    /// 
    /// </summary>

    public class FakeLogger<T> : FakeLogger, IFakeLogger<T>
    {
        public FakeLogger() : base()
        {

        }

    }


    public class FakeLogger : IFakeLogger
    {
        #region Base Methods - ignore

        public void Dispose()
        { }
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }


        #endregion

        public FakeLogger()
        {
            this.LogMessages = new List<LogMessage>();
        }


        /// <summary>
        /// Main Work
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            this.LogMessages.Add(new LogMessage
            {
                LogLevel = logLevel,
                EventId = eventId,
                Type = typeof(TState),
                Exception = exception,
                Message = formatter(state, exception)
            });

        }


        public List<LogMessage> LogMessages { get; private set; }


        /// <summary>
        /// Gets or sets the debug log.
        /// </summary>
        public List<string> DebugLog
        {
            get
            {
                return this.LogMessages.Where(a => a.LogLevel == LogLevel.Debug).Select(s => s.Message).ToList<string>();
            }
        }

        /// <summary>
        /// Gets or sets the info log.
        /// </summary>
        public List<string> InfoLog
        {
            get
            {
                return this.LogMessages.Where(a => a.LogLevel == LogLevel.Information).Select(s => s.Message).ToList<string>();
            }
        }

        /// <summary>
        /// Gets or sets the error log.
        /// </summary>
        public List<string> ErrorLog
        {
            get
            {
                return this.LogMessages.Where(a => a.LogLevel == LogLevel.Error).Select(s => s.Message).ToList<string>();
            }
        }

        public List<Exception> ExceptionLog
        {
            get
            {
                return this.LogMessages.Where(a => a.LogLevel == LogLevel.Error).Select(s => s.Exception).ToList<Exception>();
            }
        }


    }
}
