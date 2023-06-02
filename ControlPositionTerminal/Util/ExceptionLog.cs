using System;
using GBinanceFuturesClient;
using Io.Gate.GateApi.Client;

namespace ControlPositionTerminal.Util
{
    public class ExceptionLog
    {
        
        /// <summary>
        /// Метод добавляет в лог ошибку, если произошло исключение.
        /// </summary>
        /// <param name="e">Исключение.</param>
        public static void SetLogMessage(Exception e)
        {
            if (e.GetType() == typeof(ErrorMessageException))
            {
                ErrorMessageException e1 = (ErrorMessageException)e;
                LogList.AddLog(string.Format("FullErrMessage: {0} ErrMessage: {1} ErrCode: {2} HTTPStatusCode: {3}",
                    e1.Message, e1.Message, e1.Code, e1.HResult));
            }
            else if (e.GetType() == typeof(ApiException))
            {
                ApiException e1 = (ApiException)e;
                LogList.AddLog(string.Format("FullErrMessage: {0} ErrMessage: {1} ErrCode: {2} HTTPStatusCode: {3}",
                    e1.Message, e1.Message, e1.ErrorCode, e1.HResult));
            }
            else
            {
                LogList.AddLog(string.Format("FullErrMessage: {0}", e.Message));
            }
        }
    }
}