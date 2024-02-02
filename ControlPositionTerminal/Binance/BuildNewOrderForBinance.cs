using System;
using System.Globalization;
using ControlPositionTerminal.Common.Model;
using GBasicExchangeDefinitions;    
using GBinanceFuturesClient.Model.Trade;

namespace ControlPositionTerminal.Binance
{
    /// <summary>
    /// Класс формирует ордера для Binance.
    /// </summary>
    public class BuildNewOrderForBinance
    {
        /// <summary>
        /// Установить параметры.
        /// </summary>
        /// <param name="symbol">Символ.</param>
        /// <param name="side">Сторона ордера.</param>
        /// <param name="positionSide">Сторона позиции.</param>
        /// <param name="typeOrder">Тип ордера.</param>
        /// <param name="quantity">Количество.</param>
        /// <param name="price">Цена.</param>
        /// <returns>Словарь с установленными параметрами.</returns>
        public static NewOrderRequest SetParameters(string symbol, OrderSide side, PositionSide positionSide,
            OrderType typeOrder, decimal quantity, decimal price)
        {
            NewOrderRequest newOrderRequest = new NewOrderRequest()
            {
                Symbol = symbol,
                Side = side,
                PositionSide = positionSide,
                Type = typeOrder,
                Quantity = quantity,

            };
            if (price != 0)
            {
                newOrderRequest.Price = price;
            }
            return newOrderRequest;
        }

        /// <summary>
        /// Получить параметры для закрытия позиции по рыночной цене.
        /// </summary>
        /// <param name="closePos">Объект с информацией о закрываемой позиции.</param>
        /// <param name="partSize">Размер части позиции для закрытия (может быть нулевым).</param>
        /// <returns>Словарь с установленными параметрами для закрытия позиции.</returns>
        public static NewOrderRequest GetParametersForCloseOrderMarket(PositionInforamtionItem closePos, string partSize)
        {
            OrderSide orderSide = closePos.PositionAmount > 0 ? OrderSide.SELL : OrderSide.BUY;
            decimal quantity;
            decimal.TryParse(partSize, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal closingPartSize);
            if (closingPartSize != 0)
            {
                quantity = closingPartSize;
            }
            else
            {
                quantity = Math.Abs(closePos.PositionAmount);
            }

            return SetParameters(closePos.Symbol, orderSide, closePos.PositionSide, OrderType.MARKET, quantity, 0);
        }

        /// <summary>
        /// Получить параметры для установки нового значения маржинального плеча.
        /// </summary>
        /// <param name="symbol">Символ.</param>
        /// <param name="leverage">Новое значение маржинального плеча.</param>
        /// <returns>Словарь с установленными параметрами для установки нового значения маржинального плеча.</returns>
        public static InitialLeverage GetParametersForSetNewLeverageValue(string symbol, int leverage)
        {
            return new InitialLeverage()
            {
                Symbol = symbol,
                Leverage_limit = leverage,
            };
        }

    }
}