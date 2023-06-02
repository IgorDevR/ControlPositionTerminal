
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using ControlPositionTerminal.Util.Enums;

namespace ControlPositionTerminal.Util.Enums
{
    [Serializable]
    public enum TypesMarketEnum
    {

        None,
        Spot,
        Futures,
        FuturesTestnet,

    }
    public static class TypesExchanges
    {
        public static List<TypesMarketEnum> GetBinanceTypesExchanges = new List<TypesMarketEnum>
            { TypesMarketEnum.Spot, TypesMarketEnum.Futures, TypesMarketEnum.FuturesTestnet };
        public static List<TypesMarketEnum> GetGateIoTypesExchanges = new List<TypesMarketEnum>
            { TypesMarketEnum.Spot, TypesMarketEnum.Futures };


    }

 

}
