using System;
using GBinanceFuturesClient;
using Io.Gate.GateApi.Client;

namespace ControlPositionTerminal.Util
{
    public class ApplicationLog
    {

        /// <summary>
        /// Метод добавляет в лог ошибку, если произошло исключение.
        /// </summary>
        /// <param name="e">Исключение.</param>
        public static void LogException(Exception e)
        {
            string className = e.TargetSite?.DeclaringType?.FullName;
            string methodName = e.TargetSite?.Name;
            string errorMessage = e.Message;
            string stackTrace = e.StackTrace;

            if (e.GetType() == typeof(ErrorMessageException))
            {
                ErrorMessageException e1 = (ErrorMessageException)e;
                LogList.AddLog(string.Format("Class: {0} Method: {1} FullErrMessage: {2} ErrMessage: {3} ErrCode: {4} HTTPStatusCode: {5} StackTrace: {6}",
                    className, methodName, errorMessage, e1.Message, e1.Code, e1.HResult, stackTrace));
            }
            else if (e.GetType() == typeof(ApiException))
            {
                ApiException e1 = (ApiException)e;
                LogList.AddLog(string.Format("Class: {0} Method: {1} FullErrMessage: {2} ErrMessage: {3} ErrCode: {4} HTTPStatusCode: {5} StackTrace: {6}",
                    className, methodName, errorMessage, e1.Message, e1.ErrorCode, e1.HResult, stackTrace));
            }
            else
            {
                LogList.AddLog(string.Format("Class: {0} Method: {1} FullErrMessage: {2} StackTrace: {3}",
                    className, methodName, errorMessage, stackTrace));
            }
        }
    }
}