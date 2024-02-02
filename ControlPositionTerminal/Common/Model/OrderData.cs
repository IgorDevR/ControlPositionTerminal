using GBasicExchangeDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPositionTerminal.Common.Model
{
  public class OrderData
  {
    public OrderData()
    {
    }

    public OrderData(string num, string symbol, string price, string amount)
    {
      Num = num;
      Symbol = symbol;
      Price = price;
      Amount = amount;
    }
    public OrderData(string num, string symbol, string price, string amount, string orderSide, string orderId, string utcTime)
    {
      Num = num;
      Symbol = symbol;
      Price = price;
      Amount = amount;
      OrderSide = orderSide;
      OrderId = orderId;
      UTCTime = utcTime;
    }

    public string Num { get; set; }
    public string Symbol { get; set; }
    public string Price { get; set; }
    public string Amount { get; set; }
    public string OrderSide { get; set; }
    public string OrderId { get; set; }
    public string UTCTime { get; set; }


    public override string ToString()
    {
      return $"Num: {Num}, Symbol: {Symbol}, Price: {Price}, Amount: {Amount}, OrderSide: {OrderSide}";
    }
  }
}
