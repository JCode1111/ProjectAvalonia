using System;

namespace Project.Models
{
public class Transakcja
{
    public DateTime Data { get; set; }
    public decimal Kwota { get; set; }
    public string Kategoria { get; set; }
    public string Opis { get; set; }
    public string Uzytkownik { get; set; }
    public string? ZalacznikSciezka { get; set; }
}



}
