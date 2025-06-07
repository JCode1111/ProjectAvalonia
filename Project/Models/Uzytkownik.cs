using System;
using System.Collections.Generic;

public class Uzytkownik
{
    // login uzytkownika
    public string Login { get; set; }
    // haslo uzytkownika
    public string Haslo { get; set; }
    // aktualne saldo uzytkownika
    public decimal Saldo { get; set; }
    //lista limitów budżetowych przypisana dla danego uzytkownika
    public List<LimitBudzetowy> LimityBudzetowe { get; set; } = new();
}

public class LimitBudzetowy
{
    // nazwa kategorii limitu miesiecznego
    public string Kategoria { get; set; }
    //limit miesieczny ustawiony dla danego uzytkownika
    public decimal Limit { get; set; }
}
