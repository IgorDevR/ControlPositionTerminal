namespace ControlPositionTerminal.Common.Model;

/// <summary>
/// Класс, представляющий информацию об исходном плече.
/// </summary>
public class InitialLeverage

{    /// <summary>
     /// Конструктор с параметрами.
     /// </summary>
     /// <param name="symbol">символ</param>
     /// <param name="leverage_Limit">плечо</param>
    public InitialLeverage(string symbol, int leverage_Limit)
    {
        this.Symbol = symbol;
        this.Leverage_limit = leverage_Limit;
    }

    public InitialLeverage()
    {
    }
    /**
     * Символ.
     */
    public string Symbol { get; set; }

    /**
     * Плечо.
     */
    public int Leverage_limit { get; set; }

    //long MaxNotionalValue{ get; set; }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var leverage = (InitialLeverage)obj;
        return Symbol.Equals(leverage.Symbol) && Leverage_limit.Equals(leverage.Leverage_limit);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + (Symbol == null ? 0 : Symbol.GetHashCode());
        hash = hash * 23 + Leverage_limit.GetHashCode();
        return hash;
    }
}