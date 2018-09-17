using System;
using NLog;

namespace PartsReserver
{
    /// <summary>
    /// Логирование данных.
    /// </summary>
    public static class Logger
    {
        #region Fields

        /// <summary>
        /// The n logger.
        /// </summary>
        private static readonly NLog.Logger NLogger = LogManager.GetLogger("main");

        #endregion

        #region Methods

        /// <summary>
        /// Вывод данных в лог.
        /// </summary>
        /// <param name="str">
        /// Логируемое сообщение.
        /// </param>
        public static void Write(string str)
        {
            NLogger.Info(str);
        }

        /// <summary>
        /// Вывод данных в лог уровня Debug.
        /// </summary>
        /// <param name="str">
        /// Логируемое сообщение.
        /// </param>
        public static void Debug(string str)
        {
            NLogger.Debug(str);
        }

        /// <summary>
        /// Вывод данных в лог.
        /// </summary>
        /// <param name="str">
        /// Логгируемое сообщение.
        /// </param>
        /// <param name="e"> Объект ошибка.</param>
        public static void Write(string str, Exception e)
        {
            if (string.IsNullOrEmpty(str))
            {
                NLogger.Error(e.Message);
            }
            else
            {
                NLogger.Error(str + e.Message);
            }

            NLogger.Trace(e);
        }

        #endregion
    }
}