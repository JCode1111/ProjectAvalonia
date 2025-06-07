using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Project.Models;
using Project.Services;

namespace Project.Services
{
    public static class TransakcjaService
    {
        // Ścieżka do pliku JSON, w którym są zapisywane wszystkie transakcje
        private static readonly string Sciezka = Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
            "Data", "transakcje.json");

        // Zapis nowej transakcji do pliku transakcje.json
        // odczyt aktualnej listy i dodanie nowej transakcji
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

        // Wczytuje listę transakcji przypisane dla zalogowanego użytkownika
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

          // Usuwanie podanej transakcji z pliku transakcje.json      
        public static void Usun(Transakcja doUsuniecia)
        {
            if (!File.Exists(Sciezka)) return;

            var json = File.ReadAllText(Sciezka);
            var lista = JsonSerializer.Deserialize<List<Transakcja>>(json) ?? new List<Transakcja>();

            
            lista = lista
                .Where(t => !(t.Opis == doUsuniecia.Opis &&
                            t.Kwota == doUsuniecia.Kwota &&
                            t.Data == doUsuniecia.Data &&
                            t.Kategoria == doUsuniecia.Kategoria &&
                            t.Uzytkownik == doUsuniecia.Uzytkownik &&
                            t.ZalacznikSciezka == doUsuniecia.ZalacznikSciezka))
                .ToList();

            File.WriteAllText(Sciezka, JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true }));
        }

        // zmiana wybranej transakcji na nową z nowymi informacjami transakcji.
        public static void Zamien(Transakcja stara, Transakcja nowa)
        {
            if (!File.Exists(Sciezka)) return;

            var json = File.ReadAllText(Sciezka);
            var lista = JsonSerializer.Deserialize<List<Transakcja>>(json) ?? new List<Transakcja>();

            var index = lista.FindIndex(t =>
                t.Opis == stara.Opis &&
                t.Kwota == stara.Kwota &&
                t.Data == stara.Data &&
                t.Kategoria == stara.Kategoria &&
                t.Uzytkownik == stara.Uzytkownik &&
                t.ZalacznikSciezka == stara.ZalacznikSciezka
            );

            if (index >= 0)
            {
                lista[index] = nowa;
                File.WriteAllText(Sciezka, JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        
    
    }
}
