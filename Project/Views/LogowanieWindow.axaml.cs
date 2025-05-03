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
    public LogowanieWindow()
    {
        InitializeComponent();
    }

    private void Anuluj_Click(object? sender, RoutedEventArgs e) => Close(false);

    private void Zaloguj_Click(object? sender, RoutedEventArgs e)
    {
        var login = NazwaTextBox.Text?.Trim();
        var pass  = HasloTextBox.Text;

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pass))
        {
            BladTextBlock.Text = "Uzupełnij wszystkie pola.";
            return;
        }

        var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Data", "users.json");

        if (!File.Exists(path))
        {
            BladTextBlock.Text = "Brak pliku z użytkownikami.";
            return;
        }

        var users = JsonSerializer
            .Deserialize<List<Uzytkownik>>(File.ReadAllText(path))
            ?? new List<Uzytkownik>();

        var user = users.FirstOrDefault(u => u.Login == login && u.Haslo == pass);
        if (user == null)
        {
            BladTextBlock.Text = "Nieprawidłowe dane logowania.";
            return;
        }

        // Przypisujemy obiekt
        MainWindow.UzytkownikZalogowany = user;

        // Zwracamy true, by MainWindow wiedział, że jest OK
        Close(true);
    }
}

}
