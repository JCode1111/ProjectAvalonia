using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Project.Views;

namespace Project.Views
{
    public partial class MainWindow : Window
    {
        public static string? UzytkownikZalogowany { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
        }

        private async void Logowanie_Click(object? sender, RoutedEventArgs e)
        {
            var logWindow = new LogowanieWindow(this);
            await logWindow.ShowDialog(this);
            UpdateUI();
        }

        private void Rejestracja_Click(object? sender, RoutedEventArgs e)
        {
            var regWindow = new RejestracjaWindow();
            regWindow.ShowDialog(this);
            UpdateUI();
        }

        private void Wyloguj_Click(object? sender, RoutedEventArgs e)
        {
            UzytkownikZalogowany = null;
            UpdateUI();
        }

        private void UpdateUI()
        {
            bool zalogowany = !string.IsNullOrEmpty(UzytkownikZalogowany);

            // Menu items visibility
            MenuLogowanie.IsVisible = !zalogowany;
            MenuRejestracja.IsVisible = !zalogowany;
            MenuWyloguj.IsVisible = zalogowany;

            // Panels visibility
            PanelNieZalogowany.IsVisible = !zalogowany;
            PanelZalogowany.IsVisible = zalogowany;

            if (zalogowany)
            {
                PowitanieTextBlock.Text = $"Zalogowany jako: {UzytkownikZalogowany}";
                LoadHistoria();
            }
            else
            {
                // Clear history when logged out
                // HistoriaListBox.Items = new List<string>();
            }
        }

        private void LoadHistoria()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "Data", $"transakcje_{UzytkownikZalogowany}.json");
            List<string> lista;
            if (File.Exists(file))
            {
                lista = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(file)) ?? new List<string>();
            }
            else
            {
                lista = new List<string> { "Brak transakcji." };
            }
            // HistoriaListBox.Items = lista;
        }
    }
}
