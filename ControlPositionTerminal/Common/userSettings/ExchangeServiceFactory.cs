using ControlPositionTerminal.Common.UserSettings;
using ControlPositionTerminal.Util;
using ControlPositionTerminal.Util.Enums;
using Io.Gate.GateApi.Api;
using Io.Gate.GateApi.Client;
using System;
using ControlPositionTerminal.Service;
using GBinanceFuturesClient;

namespace ControlPositionTerminal.Common.userSettings
{
    [Serializable]
    public static class ExchangeServiceFactory
    {
        public static IExchangeServiceWrapper CreateExchangeService(FullNameSelectServerEnum FullNameSelectServerEnum)
        {
            try
            {
                switch (FullNameSelectServerEnum)
                {
                    // case FullNameSelectServerEnum.BINANCE_USDM_SPOT:
                    //     return new BinanceSpotServiceWrapper(new SpotClientImpl(AppSettings.Instance.ApiKey_BINANCE_SPOT, AppSettings.Instance.SecretKey_BINANCE_SPOT));
                    case FullNameSelectServerEnum.Binance_UsdM_Futures:
                        return new BinanceUMFuturesServiceWrapper(AppSettings.Instance.ApiKeyBinanceFutures, AppSettings.Instance.SecretKeyBinanceFutures, false);
                    case FullNameSelectServerEnum.Binance_CoinM_Futures:
                        return new BinanceCMFuturesServiceWrapper(AppSettings.Instance.ApiKeyBinanceFutures, AppSettings.Instance.SecretKeyBinanceFutures, false);
                    case FullNameSelectServerEnum.Binance_UsdM_FuturesTestnet:
                        return new BinanceUMFuturesServiceWrapper(AppSettings.Instance.ApiKeyBinanceFuturesTestnet, AppSettings.Instance.SecretKeyBinanceFuturesTestnet, true);
                    case FullNameSelectServerEnum.Binance_CoinM_FuturesTestnet:
                        return new BinanceCMFuturesServiceWrapper(AppSettings.Instance.ApiKeyBinanceFuturesTestnet, AppSettings.Instance.SecretKeyBinanceFuturesTestnet, true);
                    //     return new Bina(new CMFuturesClientImpl(AppSettings.Instance.ApiKey_BINANCE_FUTURES_TESTNET, AppSettings.Instance.SecretKey_BINANCE_FUTURES_TESTNET, AppSettings.Instance.TESTNET_BASE_URL));
                    // case FullNameSelectServerEnum.GATE_IO_USDM_SPOT:
                    //     return new GateIoServiceWrapper(new SpotApi(new ApiClient(AppSettings.Instance.ApiKey_GATE_IO_SPOT, AppSettings.Instance.SecretKey_GATE_IO_SPOT)));
                    case FullNameSelectServerEnum.GateIo_UsdM_Futures:
                        return new GateIoServiceFuturesWrapper(AppSettings.Instance.ApiKeyGateIoFutures, AppSettings.Instance.SecretKeyGateIoFutures);
                }
            }
            catch (Exception e)
            {
                LogList.AddLog("не удалось выполнить создание сервиса в createExchangeService() + FullNameSelectServerEnum = " + FullNameSelectServerEnum);
                Console.WriteLine(e);
                throw;
            }
            return null;
        }
    }
}
