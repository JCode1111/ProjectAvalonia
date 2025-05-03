using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Avalonia.Platform.Storage;

namespace Project.Views
{
    public partial class DodajTransakcjeWindow : Window
    {
        private string? sciezkaZalacznika;

        public DodajTransakcjeWindow()
        {
            InitializeComponent();
        }

        private async void WybierzPlik_Click(object? sender, RoutedEventArgs e)
{
    var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
    {
        Title = "Wybierz plik",
        AllowMultiple = false
    });

    if (files.Count > 0)
    {
        var file = files[0];
        sciezkaZalacznika = file.Name;
        ZalacznikTextBlock.Text = sciezkaZalacznika;
    }
}


        private void Anuluj_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Dodaj_Click(object? sender, RoutedEventArgs e)
        {
            string kwota = KwotaTextBox.Text ?? "";
            string opis = OpisTextBox.Text ?? "";
            string kategoria = (KategoriaComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Nieokreślona";

            // Możesz tu dodać logikę np. zapisania danych do bazy lub pliku
            Console.WriteLine($"Dodano transakcję: {kwota} PLN, {kategoria}, {opis}, Załącznik: {sciezkaZalacznika}");

            this.Close();
        }
    }
}
