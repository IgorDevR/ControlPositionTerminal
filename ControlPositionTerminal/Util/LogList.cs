using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ControlPositionTerminal.Util
{
    /// <summary>
    /// Класс логера, добавляет новые строки в список.
    /// </summary>
    public class LogList
    {
        private const int MAX_SIZE = 2000;
        public static readonly ObservableCollection<string> logList = new ObservableCollection<string>();

        static LogList()
        {
            logList.CollectionChanged += (sender, e) =>
            {
                while (logList.Count > MAX_SIZE)
                {
                    logList.RemoveAt(MAX_SIZE);
                }
            };
        }

        ///// <summary>
        ///// Получить список логов.
        ///// </summary>
        //public static ObservableCollection<string> GetLogList() 
        //{
        //    return logList;
        //}

        /// <summary>
        /// Добавить новую строку в лог.
        /// </summary>
        /// <param name="logMessage">Сообщение для лога.</param>
        public static void AddLog(string logMessage)
        {
            // log.Info(logMessage);
            Application.Current.Dispatcher.Invoke(() =>
            {
                logList.Insert(0, logMessage);
            });
        }
    }
}