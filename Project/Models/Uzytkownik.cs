using System;
using System.Collections.Generic;

public class Uzytkownik
{
    public string Login { get; set; }
    public string Haslo { get; set; }
    public decimal Saldo { get; set; }
    public List<LimitBudzetowy> LimityBudzetowe { get; set; } = new();
}

public class LimitBudzetowy
{
    public string Kategoria { get; set; }
    public decimal Limit { get; set; }
}
