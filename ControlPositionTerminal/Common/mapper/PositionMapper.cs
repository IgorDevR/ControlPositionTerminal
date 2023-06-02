using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ControlPositionTerminal.Common.Model;
using ControlPositionTerminal.Util.Enums;
using GBasicExchangeDefinitions;
using GBinanceFuturesClient.Model.Trade;
using Io.Gate.GateApi.Model;

namespace ControlPositionTerminal.Common.mapper
{
  public class PositionMapper
  {
    /// <summary>
    /// Преобразует объект PositionData в PositionInforamtionItem для BinanceFutures.
    /// </summary>
    /// <param name="position">Объект позиции PositionData</param>
    /// <returns>Новый объект PositionInforamtionItem</returns>
    public static PositionInforamtionItem PositionDataToPositionInforamtionItem(PositionData position)
    {
      PositionInforamtionItem dto = new PositionInforamtionItem();

      dto.Symbol = position.Symbol;
      dto.PositionAmount = decimal.Parse(position.Amount);
      dto.EntryPrice = decimal.Parse(position.EntryPrice);
      dto.PositionSide = (PositionSide)Enum.Parse(typeof (PositionSide), position.Side);

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


  }
}
