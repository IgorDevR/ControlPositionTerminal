using System;
using System.Collections.Generic;
using System.Text;

namespace GBinanceFuturesClient
{
    internal static class Config
    {
        // https://testnet.binancefuture.com/
        // http://localhost:1234/
        private const string TestNetUrl = "https://testnet.binancefuture.com/";

        private const string ApiUrl = "https://fapi.binance.com/";
        private const string ApiUrlCoinM = "https://dapi.binance.com/";

        private static string Url { get; set; } = ApiUrl;
        private static string UrlCoinM { get; set; } = ApiUrlCoinM;

        internal static bool IsTestnet { get; private set; } = false;

        internal static string ApiPublicUrl
        {
            get { return Url + "fapi/v1/"; }
        }

        internal static string ApiPublicUrlCoinM
        {
            get
            {
                if (Url == TestNetUrl)
                {
                    return Url + "dapi/v1/";
                }
                return UrlCoinM + "dapi/v1/";
            }
        }

        internal static string ApiPublicV2Url
        {
            get { return Url + "fapi/v2/"; }
        }

        internal static string ApiFuturesDataUrl
        {
            get { return Url + "futures/data/"; }
        }

        internal static string ApiPrivateUrl
        {
            get { return Url + "sapi/v1/futures"; }
        }

        internal static string ApiAccountTransferAndHistoryUrl
        {
            get { return @"https://api.binance.com/sapi/v1/futures/"; }
        }

        internal static void UseTestnet(bool use)
        {
            if (use)
            {
                Url = TestNetUrl;
                IsTestnet = true;
            }
            else
            {
                IsTestnet = false;
                Url = ApiUrl;
            }
        }
    }
}