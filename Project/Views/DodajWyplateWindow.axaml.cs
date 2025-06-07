using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Project.Views
{
    // okno do dodawnaia wyplaty zalogowanego uzytkownika
    public partial class DodajWyplateWindow : Window
    {
        public decimal Kwota { get; private set; }
        public string Miesiac { get; private set; } = string.Empty;
        public string Kategoria { get; private set; } = string.Empty;
        public string Opis { get; private set; } = string.Empty;

        public DodajWyplateWindow()
        {
            InitializeComponent();
        }

        // obsługa przycisku dodaj 
        private void Dodaj_Click(object? sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(KwotaTextBox.Text, out var kwota) && !string.IsNullOrWhiteSpace(MiesiacTextBox.Text))
            {
                Kwota = kwota;
                Miesiac = MiesiacTextBox.Text;
                Kategoria = "Wypłata";
                Opis = OpisTextBox.Text;

                // jak wszystko ok - zamyka okno 
                this.Close(true);
            }
            else
            {
                Console.WriteLine("Uzupełnij wszystkie pola.");
                this.Close(false);
            }
        }

        // obsluga przycisku anuluj - bez przekazywania danych
        private void Anuluj_Click(object? sender, RoutedEventArgs e)
        {
            this.Close(false);
        }

    }
}