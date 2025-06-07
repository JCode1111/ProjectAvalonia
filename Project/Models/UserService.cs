using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Project.Models;
using System;

namespace Project.Services
{
    public static class UserService
    {
        // sciezka do pliku json - users.json
        private static readonly string Sciezka = Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
            "Data",
            "users.json"
        );

        // Metoda aktualizująca saldo w pliku users.json
        // Znajduje uzytkownika na podstawie loginu i zapisuje zaktualizowane saldo
        public static void AktualizujSaldo(string login, decimal noweSaldo)
        {
            Console.WriteLine($"[UserService] Aktualizuję saldo dla '{login}' na {noweSaldo}");
            Console.WriteLine($"[UserService] Używana ścieżka: {Sciezka}");

            if (!File.Exists(Sciezka))
            {
                Console.WriteLine($"[UserService] Plik nie istnieje: {Sciezka}");
                return;
            }

            var json = File.ReadAllText(Sciezka);
            var lista = JsonSerializer.Deserialize<List<Uzytkownik>>(json) ?? new List<Uzytkownik>();

            var user = lista.FirstOrDefault(u => u.Login == login);
            if (user == null)
            {
                Console.WriteLine($"[UserService] Nie znaleziono użytkownika: {login}");
                return;
            }
            // aktualizacja salda
            user.Saldo = noweSaldo;

            var nowyJson = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Sciezka, nowyJson);
            Console.WriteLine("[UserService] Zaktualizowany users.json:\n" + nowyJson);
        }

        // Metoda przeliczająca saldo na podstawie sumy transakcji użytkownika
        public static void PrzeliczISaveSaldo(string login)
        {
            var transakcje = TransakcjaService.WczytajDlaUzytkownika(login);
            var noweSaldo = transakcje.Sum(t => t.Kwota);
            AktualizujSaldo(login, noweSaldo);
        }

        /// Wczytanie danych użytkownika na podstawie loginu.        
        public static Uzytkownik? Wczytaj(string login)
        {
            if (!File.Exists(Sciezka)) return null;

            var json = File.ReadAllText(Sciezka);
            var lista = JsonSerializer.Deserialize<List<Uzytkownik>>(json);
            return lista?.FirstOrDefault(u => u.Login == login);
        }

        // zapisanie uzytkownika do pliku users.json
        // gdy uzytkownik juz istnieje napisuje jego dane
        public static void Zapisz(Uzytkownik user)
        {
            if (!File.Exists(Sciezka))
                return;

            var json = File.ReadAllText(Sciezka);
            var lista = JsonSerializer.Deserialize<List<Uzytkownik>>(json) ?? new List<Uzytkownik>();

            var index = lista.FindIndex(u => u.Login == user.Login);
            if (index >= 0)
            {
                lista[index] = user;
            }
            else
            {
                lista.Add(user);
            }

            var nowyJson = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Sciezka, nowyJson);
            Console.WriteLine("[UserService] Użytkownik zapisany:\n" + nowyJson);
        }



    }
}
