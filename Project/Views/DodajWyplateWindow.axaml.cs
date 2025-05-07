using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Project.Views
{
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

        private void Dodaj_Click(object? sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(KwotaTextBox.Text, out var kwota) && !string.IsNullOrWhiteSpace(MiesiacTextBox.Text))
            {
                Kwota = kwota;
                Miesiac = MiesiacTextBox.Text;
                Kategoria = "Wypłata";
                Opis = OpisTextBox.Text;
                this.Close(true);
            }
            else
            {
                Console.WriteLine("Uzupełnij wszystkie pola.");
                this.Close(false);
            }
        }

        private void Anuluj_Click(object? sender, RoutedEventArgs e)
        {
            this.Close(false);
        }

    }
}