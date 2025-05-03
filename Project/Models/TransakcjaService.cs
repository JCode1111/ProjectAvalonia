using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Project.Models;

namespace Project.Services
{
    public static class TransakcjaService
    {
        private static readonly string Sciezka = Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
            "Data", "transakcje.json");

        public static void Zapisz(Transakcja t)
        {
            var lista = new List<Transakcja>();
            if (File.Exists(Sciezka))
            {
                try
                {
                    var json = File.ReadAllText(Sciezka);
                    lista = JsonSerializer.Deserialize<List<Transakcja>>(json) 
                            ?? new List<Transakcja>();
                }
                catch { lista = new List<Transakcja>(); }
            }
            lista.Add(t);
            File.WriteAllText(Sciezka, JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true }));
        }

        public static List<Transakcja> WczytajDlaUzytkownika(string login)
        {
            if (!File.Exists(Sciezka))
                return new List<Transakcja>();

            try
            {
                var json = File.ReadAllText(Sciezka);
                var all = JsonSerializer.Deserialize<List<Transakcja>>(json) ?? new List<Transakcja>();
                return all.Where(t => t.Uzytkownik == login).ToList();
            }
            catch
            {
                return new List<Transakcja>();
            }
        }
    }
}
