namespace WindowSill.UnitConverter.Exchange;

internal sealed record CurrencyValue(double Value, string Currency, string IsoCurrency)
{
    public string FormattedValue
    {
        get
        {
            string formattedValue = Value % 1 == 0 ? Value.ToString("N0") : Value.ToString("N2");
            return $"{formattedValue} {(IsoCurrency ?? Currency).ToUpper()}";
        }
    }
}
