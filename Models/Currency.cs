namespace QuitSmoke.Models;

public class Currency
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    
    public static List<Currency> GetAvailableCurrencies()
    {
        return new List<Currency>
        {
            new Currency { Code = "EUR", Name = "Euro", Symbol = "€" },
            new Currency { Code = "USD", Name = "Dólar Estadounidense", Symbol = "$" },
            new Currency { Code = "GBP", Name = "Libra Esterlina", Symbol = "£" },
            new Currency { Code = "JPY", Name = "Yen Japonés", Symbol = "¥" },
            new Currency { Code = "CAD", Name = "Dólar Canadiense", Symbol = "C$" },
            new Currency { Code = "AUD", Name = "Dólar Australiano", Symbol = "A$" },
            new Currency { Code = "CHF", Name = "Franco Suizo", Symbol = "CHF" },
            new Currency { Code = "CNY", Name = "Yuan Chino", Symbol = "¥" },
            new Currency { Code = "MXN", Name = "Peso Mexicano", Symbol = "$" },
            new Currency { Code = "ARS", Name = "Peso Argentino", Symbol = "$" },
            new Currency { Code = "CLP", Name = "Peso Chileno", Symbol = "$" },
            new Currency { Code = "COP", Name = "Peso Colombiano", Symbol = "$" },
            new Currency { Code = "PEN", Name = "Sol Peruano", Symbol = "S/" },
            new Currency { Code = "BRL", Name = "Real Brasileño", Symbol = "R$" }
        };
    }
}