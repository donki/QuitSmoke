using QuitSmoke.Services;

namespace QuitSmoke.Models;

public class Currency
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;

    /// <summary>Clave del nombre en los recursos de idioma (currency_EUR, currency_USD...).</summary>
    public string NameKey => $"currency_{Code}";

    /// <summary>
    /// Divisas disponibles. Con <paramref name="localization"/> el nombre sale traducido al idioma
    /// de la app; sin él (solo se necesita el símbolo) se deja el código ISO.
    /// </summary>
    public static List<Currency> GetAvailableCurrencies(ILocalizationService? localization = null)
    {
        var list = new List<Currency>
        {
            new Currency { Code = "EUR", Symbol = "€" },
            new Currency { Code = "USD", Symbol = "$" },
            new Currency { Code = "GBP", Symbol = "£" },
            new Currency { Code = "JPY", Symbol = "¥" },
            new Currency { Code = "CAD", Symbol = "C$" },
            new Currency { Code = "AUD", Symbol = "A$" },
            new Currency { Code = "CHF", Symbol = "CHF" },
            new Currency { Code = "CNY", Symbol = "¥" },
            new Currency { Code = "MXN", Symbol = "$" },
            new Currency { Code = "ARS", Symbol = "$" },
            new Currency { Code = "CLP", Symbol = "$" },
            new Currency { Code = "COP", Symbol = "$" },
            new Currency { Code = "PEN", Symbol = "S/" },
            new Currency { Code = "BRL", Symbol = "R$" }
        };

        foreach (var currency in list)
            currency.Name = localization?.GetString(currency.NameKey) ?? currency.Code;

        return list;
    }
}
