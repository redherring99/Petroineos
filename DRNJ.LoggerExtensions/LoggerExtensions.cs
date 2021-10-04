using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRNJ.LoggerExtensions
{
    /// <summary>
    /// Logger extension methods Giving Name of method calling
    /// *** Aync can mess with method names though ***
    /// TODO: Make more accurate for Async
    /// </summary>
    public static class LoggerExtensions
    {
        public static void LogTraceMsg(this ILogger logger, string message, params object[] args)
        {
            logger.LogTrace(string.Format("{0}: {1}", MethodName.CallingMethodFrame(1), message), args);
        }

        public static void LogDebugMsg(this ILogger logger, string message, params object[] args)
        {
            logger.LogDebug(string.Format("{0}: {1}", MethodName.CallingMethodFrame(1), message), args);
        }

        public static void LogInfoMsg(this ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(string.Format("{0}: {1}", MethodName.CallingMethodFrame(1), message), args);
        }

        public static void LogWarningMsg(this ILogger logger, string message, params object[] args)
        {
            logger.LogWarning(string.Format("{0}: {1}", MethodName.CallingMethodFrame(1), message), args);
        }

        public static void LogCriticalMsg(this ILogger logger, string message, params object[] args)
        {
            logger.LogCritical(string.Format("{0}: {1}", MethodName.CallingMethodFrame(1), message), args);
        }

        public static void LogErrorMsg(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.LogError(exception, string.Format("{0}: {1}", MethodName.CallingMethodFrame(1), message), args);
        }

        public static void LogErrorMsg(this ILogger logger, string message, params object[] args)
        {
            logger.LogError(string.Format("{0}: {1}", MethodName.CallingMethodFrame(1), message), args);
        }

    }
}
