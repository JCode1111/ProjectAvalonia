using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System;
using Project.Models; // Dodaj przestrzeń nazw, w której znajduje się klasa Uzytkownik

namespace Project.Views
{
    public partial class LogowanieWindow : Window
    {
        private readonly MainWindow _mainWindow;

        public LogowanieWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Anuluj_Click(object? sender, RoutedEventArgs e) => this.Close();

        private void Zaloguj_Click(object? sender, RoutedEventArgs e)
        {
            // Pobierz pełną ścieżkę do pliku users.json w folderze Data
          string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Data", "users.json");


            if (!File.Exists(path))
            {
                BladTextBlock.Text = "Brak pliku z użytkownikami.";
                return;
            }

            var nazwa = NazwaTextBox.Text?.Trim();
            var haslo = HasloTextBox.Text;

            if (string.IsNullOrWhiteSpace(nazwa) || string.IsNullOrWhiteSpace(haslo))
            {
                BladTextBlock.Text = "Uzupełnij wszystkie pola.";
                return;
            }

            var users = JsonSerializer.Deserialize<List<Uzytkownik>>(File.ReadAllText(path)) ?? new List<Uzytkownik>();
            var user = users.FirstOrDefault(u => u.Nazwa == nazwa && u.Haslo == haslo);

            if (user != null)
            {
                // Zalogowano pomyślnie - ustawienie zalogowanego użytkownika w MainWindow
                MainWindow.UzytkownikZalogowany = nazwa!;
            //    _mainWindow.AktualizujPowitanie();
                this.Close();
            }
            else
            {
                BladTextBlock.Text = "Nieprawidłowe dane logowania.";
            }
        }
    }
}
