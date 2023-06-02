using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ControlPositionTerminal.Binance;
using ControlPositionTerminal.Common;
using ControlPositionTerminal.Common.mapper;
using ControlPositionTerminal.Common.Model;
using ControlPositionTerminal.Exceptions;
using ControlPositionTerminal.Util;
using GBinanceFuturesClient;
using GBinanceFuturesClient.Model.Trade;

namespace ControlPositionTerminal.Service;

[Serializable]
public class BinanceUMFuturesServiceWrapper : IExchangeServiceWrapper
{
    private readonly BinanceFuturesClient _client;
    public BinanceUMFuturesServiceWrapper(string key, string secret, bool isUseTestnet)
    {
        this._client = new BinanceFuturesClient();
        _client.SetAutorizationData(key, secret);
        _client.UseTestnet(isUseTestnet);
    }

    public async Task<List<PositionData>> RetrieveOpenPositionsAsync()
    {
        List<PositionInforamtionItem> positions = await Task.Run(() =>  ExecuteWithExceptionHandling.Execute(() => _client.Trade.GetPostionsInformation()));
        return GetOnlyOpenPositionDataFromResult(positions);
    }

    public async Task<List<InitialLeverage>> RetrieveAllLeverageCoinsAsync()
    {
        List<PositionInforamtionItem> positions = await Task.Run(() => ExecuteWithExceptionHandling.Execute(() => _client.Trade.GetPostionsInformation()));

        List<InitialLeverage> nameAndLeverageAllCoins = GetNameAndLeverageAllCoins(positions);
        return nameAndLeverageAllCoins;
    }

    public async Task CloseSelectPositionAsync(PositionData positionForClose, string partSize)
    {
        PositionInforamtionItem PositionInforamtionItem = PositionMapper.PositionDataToPositionInforamtionItem(positionForClose);
        NewOrderRequest parameterCloseOrderMarket = BuildNewOrderForBinance.GetParametersForCloseOrderMarket(PositionInforamtionItem, partSize);
        OrderInfo orderInfo = await Task.Run(() =>  ExecuteWithExceptionHandling.Execute(() => _client.Trade.PlaceOrder(parameterCloseOrderMarket)));
        LogList.AddLog("Выполено закрытие позиции. " + orderInfo.OrigQty);
    }

    public async Task SetNewValueLeverage<T>(List<T> symbols, string leverageStr)
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
                LogList.AddLog("Изменение плеча у всех символов. Пожалуйста ожидайте... "+ "Текущий: " + symbolAndLeverage.Symbol + ".Осталось: " + (symbols.Count - i++));
            }
            await Task.Run(() => ExecuteWithExceptionHandling.Execute(() => _client.Trade.ChangeLeverage(symbolAndLeverage.Symbol, leverage)));
        }
        LogList.AddLog("Изменение плечей завершено.");
    }

    public List<PositionData> GetOnlyOpenPositionDataFromResult<T>(List<T> positions)
    {;
        List<PositionData> positionData = PositionMapper.BinancePositionsToPositionData(positions);
        positionData = PositionUtils.GetOpenPositions(positionData);

        return positionData;
    }

    public List<InitialLeverage> GetNameAndLeverageAllCoins(List<PositionInforamtionItem> result)
    {
        List<InitialLeverage> symbolAndLeverage = result
            .Select(dto => new InitialLeverage(dto.Symbol, dto.Leverage))
            .Distinct()
            .ToList();
        return symbolAndLeverage;
    }


}