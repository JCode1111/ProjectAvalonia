using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;

namespace Project.Views
{
    public partial class DodajTransakcjeWindow : Window
    {
        public decimal Kwota { get; private set; }
        public string Kategoria { get; private set; } = string.Empty;
        public string Opis { get; private set; } = string.Empty;
        public string? ZalacznikSciezka { get; private set; }

        public DodajTransakcjeWindow()
        {
            InitializeComponent();
        }

        private async void WybierzPlik_Click(object? sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false
            };

            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                ZalacznikSciezka = result[0];
                ZalacznikTextBlock.Text = Path.GetFileName(ZalacznikSciezka);
            }
        }

        private void Dodaj_Click(object? sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(KwotaTextBox.Text, out var kwota))
            {
                Kwota = -kwota;
                Kategoria = (KategoriaComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Inne";
                Opis = OpisTextBox.Text;

                this.Close(true); // success
            }
            else
            {
               Console.WriteLine("Uzupełnij wszystkie pola!"); // tylko do debugowania
                this.Close(false); // lub nie zamykaj w ogóle
            }
        }

        private void Anuluj_Click(object? sender, RoutedEventArgs e)
        {
            this.Close(false); // cancelled
        }
    }
}
