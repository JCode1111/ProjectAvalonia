using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Project.Views;

namespace Project.Views
{
    public partial class MainWindow : Window
    {
        public static string? UzytkownikZalogowany { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            // Ustawienie tekstu powitania na początku, jeżeli użytkownik jest już zalogowany
            AktualizujPowitanie();
        }

        private void Logowanie_Click(object? sender, RoutedEventArgs e)
        {
            var logowanieWindow = new LogowanieWindow(this); // Przekazujemy referencję do MainWindow
            logowanieWindow.ShowDialog(this);
        }

        private void Rejestracja_Click(object? sender, RoutedEventArgs e)
        {
            var rejestracjaWindow = new RejestracjaWindow();
            rejestracjaWindow.ShowDialog(this);
        }

        private void DodajTransakcje_Click(object? sender, RoutedEventArgs e)
        {
            // Logika dodawania transakcji
        }

        public void AktualizujPowitanie() // Zmieniamy metodę na publiczną
        {
            // Sprawdzenie, czy użytkownik jest zalogowany
            if (!string.IsNullOrEmpty(UzytkownikZalogowany))
            {
                PowitanieTextBlock.Text = $"Witaj użytkowniku: {UzytkownikZalogowany}";
            }
            else
            {
                PowitanieTextBlock.Text = "Nie jesteś zalogowany.";
            }
        }
    }
}
