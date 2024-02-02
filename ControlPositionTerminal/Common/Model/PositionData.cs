using System.Diagnostics;

namespace ControlPositionTerminal.Common.Model
{
  /// <summary>
  /// Класс содержит общую информацию для позиций различных бирж. Используется так же для вывода на экран в jTable.
  /// </summary>
  public class PositionData {
    public PositionData()
    {
    }

    public PositionData(string num, string symbol, string entryPrice, string amount, string side)
    {
      Num = num;
      Symbol = symbol;
      EntryPrice = entryPrice;
      Amount = amount;
      Side = side;
    }

    public string Num { get; set; }
    public string Symbol { get; set; }
    public string EntryPrice { get; set; }
    public string Amount { get; set; }
    public string Side { get; set; }

    public override string ToString()
    {
      return $"Num: {Num}, Symbol: {Symbol}, Price: {EntryPrice}, Amount: {Amount}, Side: {Side}";
    }

  }
}
