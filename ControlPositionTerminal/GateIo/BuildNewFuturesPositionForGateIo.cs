using Io.Gate.GateApi.Model;
using System.Diagnostics;

namespace ControlPositionTerminal.GateIo
{
    public class BuildNewFuturesPositionForGateIo
    {
        /// <summary>
        /// Собирает новый ордер для закрытия позиции на рынке.
        /// </summary>
        /// <param name="position">Существующая позиция, которую нужно закрыть</param>
        /// <param name="partSize">Размер закрываемой позиции</param>
        /// <returns>Ордер для закрытия или уменьшения позиции</returns>
        public static FuturesOrder BuildCloseOrderMarket(FuturesOrder position, string partSize)
        {
            FuturesOrder futuresOrder = new FuturesOrder(position.Contract, 0)
            {
                Price = "0",
                Tif = (FuturesOrder.TifEnum?)BatchFuturesOrder.TifEnum.Ioc,
                ReduceOnly = true,
                Close = false
            };

            if (partSize != "0")
            {
                futuresOrder.Size = position.Size > 0 ? long.Parse(partSize) * -1 : long.Parse(partSize);
            }
            else
            {
                futuresOrder.AutoSize = (FuturesOrder.AutoSizeEnum?)(position.Size > 0 ? BatchFuturesOrder.AutoSizeEnum.Long : BatchFuturesOrder.AutoSizeEnum.Short);
            }

            return futuresOrder;
        }
    }
}