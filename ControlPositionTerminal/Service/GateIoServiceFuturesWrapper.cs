using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using ControlPositionTerminal.Common;
using ControlPositionTerminal.Common.Mapper;
using ControlPositionTerminal.Common.Model;
using ControlPositionTerminal.Exceptions;
using ControlPositionTerminal.GateIo;
using ControlPositionTerminal.Util;
using GBinanceFuturesClient.Model.Trade;
using Io.Gate.GateApi.Api;
using Io.Gate.GateApi.Client;
using Io.Gate.GateApi.Model;

namespace ControlPositionTerminal.Service
{
    [Serializable]
    public class GateIoServiceFuturesWrapper : IExchangeServiceWrapper
    {
        private readonly FuturesApi _client;
        private string _settle = "usdt";
        private string _orderStatus = "open";

        public GateIoServiceFuturesWrapper(string key, string secret)
        {
            Configuration configuration = new Configuration();
            configuration.ApiV4Key = key;
            configuration.ApiV4Secret = secret;
            this._client = new FuturesApi(configuration);
        }

        public async Task<List<PositionData>> RetrieveOpenPositionsAsync()
        {
            List<Position> positions = new List<Position>();
            try
            {
                positions = await ExecuteWithExceptionHandling.Execute(() => _client.ListPositionsAsync(_settle));
            }
            catch (Exception e)
            {
                LogList.AddLog(
                    $"Error {MethodBase.GetCurrentMethod()?.DeclaringType?.Name} -  {MethodBase.GetCurrentMethod()?.Name}. ErrorMessage: " +
                    e.Message);
            }

            return GetOnlyOpenPositionDataFromResult(positions);
        }

        public async Task<List<InitialLeverage>> RetrieveAllLeverageCoinsAsync()
        {
            List<Position> positions = new List<Position>();
            try
            {
                positions = ExecuteWithExceptionHandling.Execute(() => _client.ListPositions(_settle));
            }
            catch (Exception e)
            {
                LogList.AddLog(
                    $"Error {MethodBase.GetCurrentMethod()?.DeclaringType?.Name} -  {MethodBase.GetCurrentMethod()?.Name}. ErrorMessage: " +
                    e.Message);
            }

            List<InitialLeverage> nameAndLeverageAllCoins = GetNameAndLeverageAllCoins(positions);
            return nameAndLeverageAllCoins;
        }

        public async Task CloseSelectPositionAsync(List<PositionData> closePosList, string partSize)
        {
            foreach (var closePos in closePosList)
            {
                FuturesOrder futuresOrder = PositionMapper.PositionDataToGateIoFuturesOrder(closePos);
                FuturesOrder closeOrderMarket =
                    BuildNewFuturesPositionForGateIo.BuildCloseOrderMarket(futuresOrder, partSize);

                FuturesOrder orderResponse = new FuturesOrder();

                try
                {
                    orderResponse =
                        await ExecuteWithExceptionHandling.Execute(() =>
                            _client.CreateFuturesOrderAsync("usdt", closeOrderMarket));
                    if (orderResponse != null)
                    {
                        LogList.AddLog("Закрытие позиции: " + closePos + ". Статус: " + orderResponse.Status);
                    }
                }
                catch (Exception e)
                {
                    LogList.AddLog(
                        $"Error {MethodBase.GetCurrentMethod()?.DeclaringType?.Name} -  {MethodBase.GetCurrentMethod()?.Name}. ErrorMessage: " +
                        e.Message);
                }
            }
        }

        public async Task SetNewValueLeverage<T>(List<T> symbols, string newLeverage)
        {
            int i = 0;
            foreach (InitialLeverage symbolAndLeverage in symbols.Cast<InitialLeverage>())
            {
                if (newLeverage == "0")
                {
                    LogList.AddLog($"Неверное значение плеча.");
                    continue;
                }

                if (symbolAndLeverage.Leverage_limit.ToString() == newLeverage)
                {
                    LogList.AddLog($"Для символа: {symbolAndLeverage.Symbol}. Плечо уже равно: {newLeverage}");
                    continue;
                }

                if (symbols.Count > 1)
                {
                    LogList.AddLog("Изменение плеча у всех символов. Пожалуйста ожидайте... " + "Текущий: " +
                                   symbolAndLeverage.Symbol + ".Осталось: " + (symbols.Count - i++));
                }

                try
                {
                    await ExecuteWithExceptionHandling.Execute(() =>
                        _client.UpdateDualModePositionLeverageAsync(_settle, symbolAndLeverage.Symbol, "0",
                            newLeverage));
                }
                catch (Exception e)
                {
                    LogList.AddLog(
                        $"Error {MethodBase.GetCurrentMethod()?.DeclaringType?.Name} -  {MethodBase.GetCurrentMethod()?.Name}. ErrorMessage: " +
                        e.Message);
                }
            }
        }

        public List<PositionData> GetOnlyOpenPositionDataFromResult<T>(List<T> positions)
        {
            List<PositionData> positionData = PositionMapper.GateIoPositionsToPositionData(positions);
            positionData = PositionUtils.GetOpenPositions(positionData);
            return positionData;
        }

        public List<InitialLeverage> GetNameAndLeverageAllCoins(List<Position> result)
        {
            //List<PositionInforamtionItem> binancePosDto = Parser.ParseJsonToList<PositionInforamtionItem>((string)result[0]);
            List<InitialLeverage> symbolAndLeverage = result
                .Select(dto => new InitialLeverage(dto.Contract, Convert.ToInt32(dto.CrossLeverageLimit)))
                .Distinct()
                .ToList();
            return symbolAndLeverage;
        }

        public async Task<List<OrderData>> RetrieveAllOpenOrdersAsync()
        {
            List<InitialLeverage> allCoins = await RetrieveAllLeverageCoinsAsync();

            List<OrderData> openOrders = new List<OrderData>();
            List<FuturesOrder> futuresOrders = new List<FuturesOrder>();
            foreach (var coin in allCoins)
            {
                try
                {
                    List<FuturesOrder> order = await ExecuteWithExceptionHandling.Execute(() =>
                        _client.ListFuturesOrdersAsync(_settle, coin.Symbol, _orderStatus));
                    futuresOrders.AddRange(order);
                }
                catch (Exception e)
                {
                    LogList.AddLog(
                        $"Error {MethodBase.GetCurrentMethod()?.DeclaringType?.Name} -  {MethodBase.GetCurrentMethod()?.Name}. ErrorMessage: " +
                        e.Message);
                }
            }

            openOrders = PositionMapper.GateIoOrdersToPositionData(futuresOrders);
            return openOrders;
        }

        public async Task CancelOpenOrdersAsync(List<OrderData> orderDatas)
        {
            foreach (var order in orderDatas)
            {
                try
                {
                    FuturesOrder futuresOrder =
                        await ExecuteWithExceptionHandling.Execute(() =>
                            _client.CancelFuturesOrderAsync(_settle, order.OrderId));
                    if (futuresOrder.Status != null)
                    {
                        LogList.AddLog("Отмена ордера: " + order + ". Статус: " + futuresOrder.Status);
                    }
                }
                catch (Exception e)
                {
                    LogList.AddLog(
                        $"Error {MethodBase.GetCurrentMethod()?.DeclaringType?.Name} -  {MethodBase.GetCurrentMethod()?.Name}. ErrorMessage: " +
                        e.Message);
                }
            }
        }
    }
}