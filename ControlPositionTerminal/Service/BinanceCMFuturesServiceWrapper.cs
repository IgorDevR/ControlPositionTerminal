using System;
using ControlPositionTerminal.Common.Model;
using GBinanceFuturesClient.Model.Trade;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ControlPositionTerminal.Exceptions;
using GBinanceFuturesClient;
using Io.Gate.GateApi.Model;
using ControlPositionTerminal.Util;
using ControlPositionTerminal.Binance;
using ControlPositionTerminal.Common.Mapper;
using System.Linq;

namespace ControlPositionTerminal.Service;

public class BinanceCMFuturesServiceWrapper : BinanceUMFuturesServiceWrapper
{
    public BinanceCMFuturesServiceWrapper(string key, string secret, bool isUseTestnet) : base(key, secret,
        isUseTestnet)
    {
    }

    public override async Task<List<PositionData>> RetrieveOpenPositionsAsync()
    {
        List<PositionInforamtionItem> positions = new List<PositionInforamtionItem>();
        try
        {
            positions = await Task.Run(() =>
                ExecuteWithExceptionHandling.Execute(() => _client.Trade.GetPositionsInformation(true)));
        }
        catch (Exception e)
        {
            LogList.AddLog(
                $"Error {MethodBase.GetCurrentMethod()?.DeclaringType?.Name} -  {MethodBase.GetCurrentMethod()?.Name}. ErrorMessage: " +
                e.Message);
        }

        return GetOnlyOpenPositionDataFromResult(positions);
    }

    public override async Task<List<InitialLeverage>> RetrieveAllLeverageCoinsAsync()
    {
        List<PositionInforamtionItem> positions = new List<PositionInforamtionItem>();
        try
        {
            positions = await Task.Run(() =>
                ExecuteWithExceptionHandling.Execute(() => _client.Trade.GetPositionsInformation(true)));
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

    public override async Task CloseSelectPositionAsync(List<PositionData> closePositions, string partSize)
    {
        foreach (var closePos in closePositions)
        {
            PositionInforamtionItem PositionInforamtionItem =
                PositionMapper.PositionDataToPositionInforamtionItem(closePos);
            NewOrderRequest parameterCloseOrderMarket =
                BuildNewOrderForBinance.GetParametersForCloseOrderMarket(PositionInforamtionItem, partSize);

            OrderInfo orderInfo = new OrderInfo();

            try
            {
                orderInfo = await Task.Run(() =>
                    ExecuteWithExceptionHandling.Execute(() =>
                        _client.Trade.PlaceOrder(parameterCloseOrderMarket, true)));
                await Task.Delay(2000);
                OrderInfo info = await Task.Run(() =>
                    ExecuteWithExceptionHandling.Execute(() =>
                        _client.Trade.GetOrder(orderInfo.Symbol, orderInfo.OrderId, 10000, true)));
                if (info != null)
                {
                    LogList.AddLog("Закрытие позиции: " + closePos + ". Закрытый размер: " + info.ExecutedQty);
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

    public override async Task SetNewValueLeverage<T>(List<T> symbols, string leverageStr)
    {
        int.TryParse(leverageStr, out int leverage);
        int i = 0;
        foreach (InitialLeverage symbolAndLeverage in symbols.Cast<InitialLeverage>())
        {
            if (leverage == 0)
            {
                LogList.AddLog($"Неверное значение плеча.");
                continue;
            }

            if (symbolAndLeverage.Leverage_limit == leverage)
            {
                LogList.AddLog($"Для символа: {symbolAndLeverage.Symbol}. Плечо уже равно: {leverage}");
                continue;
            }

            if (symbols.Count > 1)
            {
                LogList.AddLog("Изменение плеча у всех символов. Пожалуйста ожидайте... " + "Текущий: " +
                               symbolAndLeverage.Symbol + ".Осталось: " + (symbols.Count - i++));
            }

            try
            {
                await Task.Run(() =>
                    ExecuteWithExceptionHandling.Execute(() =>
                        _client.Trade.ChangeLeverage(symbolAndLeverage.Symbol, leverage, isCoinM: true)));
            }
            catch (Exception e)
            {
                LogList.AddLog(
                    $"Error {MethodBase.GetCurrentMethod()?.DeclaringType?.Name} -  {MethodBase.GetCurrentMethod()?.Name}. ErrorMessage: " +
                    e.Message);
            }
        }
    }

    public override List<PositionData> GetOnlyOpenPositionDataFromResult<T>(List<T> positions)
    {
        List<PositionData> positionData = PositionMapper.BinancePositionsToPositionData(positions);
        positionData = PositionUtils.GetOpenPositions(positionData);

        return positionData;
    }

    public override List<InitialLeverage> GetNameAndLeverageAllCoins(List<PositionInforamtionItem> result)
    {
        List<InitialLeverage> symbolAndLeverage = result
            .Select(dto => new InitialLeverage(dto.Symbol, dto.Leverage))
            .Distinct()
            .ToList();
        return symbolAndLeverage;
    }

    public override async Task<List<OrderData>> RetrieveAllOpenOrdersAsync()
    {
        try
        {
            List<OrderInfo> orders = await Task.Run(() =>
                ExecuteWithExceptionHandling.Execute(() => _client.Trade.GetAllOpenOrders(true)));

            List<OrderData> binanceOrdersToPositionData = PositionMapper.BinanceOrdersToPositionData(orders);
            return binanceOrdersToPositionData;
        }
        catch (Exception e)
        {
            LogList.AddLog(
                $"Error {MethodBase.GetCurrentMethod()?.DeclaringType?.Name} -  {MethodBase.GetCurrentMethod()?.Name}. ErrorMessage: " +
                e.Message);
        }

        return new List<OrderData> { };
    }

    public override async Task CancelOpenOrdersAsync(List<OrderData> orderDatas)
    {
        foreach (var order in orderDatas)
        {
            OrderInfo orderInfo = new OrderInfo();
            try
            {
                orderInfo = await Task.Run(() =>
                    ExecuteWithExceptionHandling.Execute(() =>
                        _client.Trade.CancelOrder(order.Symbol, long.Parse(order.OrderId), 10000, true)));
                if (orderInfo != null && orderInfo.Status != null)
                {
                    LogList.AddLog("Отмена ордера: " + order + ". Статус: " + orderInfo.Status);
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