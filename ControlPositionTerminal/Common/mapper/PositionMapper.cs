using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ControlPositionTerminal.Common.Model;
using ControlPositionTerminal.Util.Enums;
using GBasicExchangeDefinitions;
using GBinanceFuturesClient.Model.Trade;
using Io.Gate.GateApi.Model;

namespace ControlPositionTerminal.Common.Mapper
{
  public class PositionMapper
  {
    #region position
    /// <summary>
    /// Преобразует объект PositionData в PositionInforamtionItem для BinanceFutures.
    /// </summary>
    /// <param name="position">Объект позиции PositionData</param>
    /// <returns>Новый объект PositionInforamtionItem</returns>
    public static PositionInforamtionItem PositionDataToPositionInforamtionItem(PositionData position)
    {
      PositionInforamtionItem dto = new PositionInforamtionItem();
      if (position.Amount == "нет")
      {
        return dto;
      }

      dto.Symbol = position.Symbol;
      dto.PositionAmount = decimal.Parse(position.Amount);
      dto.EntryPrice = decimal.Parse(position.EntryPrice);
      dto.PositionSide = (PositionSide)Enum.Parse(typeof(PositionSide), position.Side);

      return dto;
    }

    /// <summary>
    /// Преобразует объект PositionData в FuturesOrder для GateIo.
    /// </summary>
    /// <param name="position">Объект позиции PositionData</param>
    /// <returns>Новый объект FuturesOrder для GateIo</returns>
    public static FuturesOrder PositionDataToGateIoFuturesOrder(PositionData position)
    {
      string positionSymbol = position.Symbol;
      long size = long.Parse(position.Amount);
      FuturesOrder futuresOrder = new FuturesOrder(positionSymbol, size);

      return futuresOrder;
    }

    /// <summary>
    /// Преобразует список объектов PositionInforamtionItem в список объектов PositionData.
    /// </summary>
    /// <param name="positions">Список позиций BinanceFutures</param>
    /// <returns>Список позиций общего вида PositionData</returns>
    public static List<PositionData> BinancePositionsToPositionData<T>(List<T> positions)
    {
      List<PositionData> dtoList = new List<PositionData>();
      int posCnt = 1;
      foreach (PositionInforamtionItem position in positions.Cast<PositionInforamtionItem>())
      {
        decimal positionAmt = position.PositionAmount;
        if (positionAmt.CompareTo(decimal.Zero) == 0)
        {
          continue;
        }
        PositionData dto = new PositionData();
        dto.Num = posCnt++.ToString();
        dto.Symbol = position.Symbol;
        dto.Amount = position.PositionAmount.ToString();
        dto.EntryPrice = position.EntryPrice.ToString(CultureInfo.CurrentCulture);
        dto.Side = position.PositionSide.ToString();
        dtoList.Add(dto);
      }

      return dtoList;
    }

    /// <summary>
    /// Преобразует список объектов Position в список объектов PositionData для GateIo.
    /// </summary>
    /// <param name="positions">Список позиций GateIo</param>
    /// <returns>Список позиций общего вида PositionData</returns>
    public static List<PositionData> GateIoPositionsToPositionData<T>(List<T> positions)
    {

      List<PositionData> dtoList = new List<PositionData>();
      int posCnt = 1;
      foreach (Position position in positions.Cast<Position>())
      {
        if (position.Size == 0)
        {
          continue;
        }
        PositionData dto = new PositionData();
        dto.Num = posCnt++.ToString();
        dto.Symbol = position.Contract;
        dto.Amount = position.Size.ToString();
        dto.EntryPrice = position.EntryPrice;
        dto.Side = position.Size > 0 ? PositionSideEnum.LONG.ToString()
          : PositionSideEnum.SHORT.ToString();
        dtoList.Add(dto);
      }
      return dtoList;
    }
    #endregion

    #region order
    /// <summary>
    /// Преобразует список объектов PositionInforamtionItem в список объектов PositionData.
    /// </summary>
    /// <param name="orders">Список позиций BinanceFutures</param>
    /// <returns>Список позиций общего вида PositionData</returns>
    public static List<OrderData> BinanceOrdersToPositionData<T>(List<T> orders)
    {
      List<OrderData> dtoList = new List<OrderData>();
      int posCnt = 1;
      foreach (OrderInfo order in orders.Cast<OrderInfo>())
      {

        OrderData dto = new OrderData();
        dto.Num = posCnt++.ToString();
        dto.Symbol = order.Symbol;
        dto.Price = order.Price.ToString(CultureInfo.CurrentCulture);
        dto.Amount = order.OrigQty.ToString();
        dto.OrderSide = order.Side.ToString();
        dto.OrderId = order.OrderId.ToString();
        dto.UTCTime = new DateTime(1970, 1, 1).AddMilliseconds(order.Time).ToString();
        dtoList.Add(dto);
      }

      return dtoList;
    }

    /// <summary>
    /// Преобразует список объектов Position в список объектов PositionData для GateIo.
    /// </summary>
    /// <param name="orders">Список позиций GateIo</param>
    /// <returns>Список позиций общего вида PositionData</returns>
    public static List<OrderData> GateIoOrdersToPositionData<T>(List<T> orders)
    {

      List<OrderData> dtoList = new List<OrderData>();
      int posCnt = 1;
      foreach (FuturesOrder order in orders.Cast<FuturesOrder>())
      {
        OrderData dto = new OrderData();
        dto.Num = posCnt++.ToString();
        dto.Symbol = order.Contract;
        dto.Price = order.Price;
        dto.Amount = order.Size.ToString();
        dto.OrderSide = order.Size > 0 ? PositionSideEnum.LONG.ToString() : PositionSideEnum.SHORT.ToString();
        dto.OrderId = order.Id.ToString();
        dto.UTCTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(order.CreateTime).ToString();
        dtoList.Add(dto);
      }
      return dtoList;
    }
    #endregion

  }
}
