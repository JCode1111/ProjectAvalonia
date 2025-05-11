using System;

namespace Project.Models
{
    public class TransakcjaWidok
    {
        public int Index { get; set; }
        public string Wyswietlacz { get; set; } = string.Empty;
        public Transakcja OryginalnaTransakcja { get; set; } = null!;
    }
}