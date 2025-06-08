using Avalonia.Controls;
using Avalonia.Interactivity;
using Project.Models; // Dodaj przestrzeń nazw do klasy Uzytkownik
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System;

namespace Project.Views
{
    /// Okno rejestracji nowego użytkownika.
    public partial class RejestracjaWindow : Window
    {
        public RejestracjaWindow()
        {
            InitializeComponent();
        }
        /// Obsługa przycisku "Anuluj" — zamyka okno rejestracji.
        private void Anuluj_Click(object? sender, RoutedEventArgs e) => this.Close();

        /// Obsługa przycisku "Zarejestruj".
        private void Zarejestruj_Click(object? sender, RoutedEventArgs e)
        {

            var nazwa = NazwaTextBox.Text?.Trim();
            var haslo = HasloTextBox.Text;

            // pola muszą być uzupełnione
            if (string.IsNullOrWhiteSpace(nazwa) || string.IsNullOrWhiteSpace(haslo))
            {
                BladTextBlock.Text = "Uzupełnij wszystkie pola.";
                return;
            }

            string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Data", "users.json");

            List<Uzytkownik> users;
            if (File.Exists(path))
            {
                try
                {
                    var jsonData = File.ReadAllText(path);
                    users = JsonSerializer.Deserialize<List<Uzytkownik>>(jsonData) ?? new List<Uzytkownik>();
                    Console.WriteLine($"Załadowano {users.Count} użytkowników.");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Błąd deserializacji: {ex.Message}");
                    users = new List<Uzytkownik>(); 
                }
            }
            else
            {
                // Plik nie istnieje — tworzymy katalog i pustą listę użytkowników
                Console.WriteLine("Plik nie istnieje, tworzymy nową listę.");
                Directory.CreateDirectory("Data");
                users = new List<Uzytkownik>(); // Nowa lista użytkowników, jeśli plik nie istnieje
            }

            // Sprawdzanie, czy użytkownik o danej nazwie już istnieje
            if (users.Exists(u => u.Login == nazwa))
            {
                BladTextBlock.Text = "Taki użytkownik już istnieje.";
                return;
            }

            // Dodanie nowego użytkownika do listy
            users.Add(new Uzytkownik { Login = nazwa!, Haslo = haslo });
            Console.WriteLine("Nowy użytkownik dodany.");

             // Zapisanie zaktualizowanej listy użytkowników do pliku
            try
            {
                var jsonString = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(path, jsonString);
                Console.WriteLine($"Dane zapisane do pliku: {path}");
            }
            catch (IOException ex)
            {
                BladTextBlock.Text = "Wystąpił błąd przy zapisie do pliku: " + ex.Message;
                return;
            }

            MainWindow.UzytkownikZalogowany = new Uzytkownik { Login = nazwa!, Haslo = haslo };
            this.Close();
        }
    }
}
