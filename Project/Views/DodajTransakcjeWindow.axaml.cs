using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.ComponentModel;
using System.IO;
using Project.Services;
using Project.Models;


namespace Project.Views
{
    public partial class DodajTransakcjeWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public decimal Kwota { get; set; }
        public string Kategoria { get; set; } = "";
        public string Opis { get; set; } = "";
        public string? ZalacznikSciezka { get; set; }

        private Transakcja? EdytowanaTransakcja = null;

        private string _tytulOkna = "Dodaj transakcję";
        public string TytulOkna
        {
            get => _tytulOkna;
            set
            {
                _tytulOkna = value;
                OnPropertyChanged(nameof(TytulOkna));
            }
        }

        private string _tekstPrzycisku = "Dodaj";
        public string TekstPrzycisku
        {
            get => _tekstPrzycisku;
            set
            {
                _tekstPrzycisku = value;
                OnPropertyChanged(nameof(TekstPrzycisku));
            }
        }
    public DodajTransakcjeWindow()
{
    InitializeComponent();
    DataContext = this;
    this.Loaded += Window_Loaded;
}

public DodajTransakcjeWindow(Transakcja? doEdycji = null)
{
    InitializeComponent();
    DataContext = this;

    if (doEdycji != null)
    {
        EdytowanaTransakcja = doEdycji;
        TytulOkna = "Edytuj transakcję";
        TekstPrzycisku = "Zapisz";

        Kwota = Math.Abs(doEdycji.Kwota); // tylko do edycji — pokazuje dodatnio
        Opis = doEdycji.Opis;
        Kategoria = doEdycji.Kategoria;
        ZalacznikSciezka = doEdycji.ZalacznikSciezka;
    }
    else
    {
        TytulOkna = "Dodaj transakcję";
        TekstPrzycisku = "Dodaj";
    }
}


public Transakcja ZwrocTransakcje(string login)
{
    return new Transakcja
    {
        Kwota = EdytowanaTransakcja?.Kwota < 0 ? -Math.Abs(Kwota) : Kwota,
        Opis = Opis,
        Kategoria = Kategoria,
        ZalacznikSciezka = ZalacznikSciezka,
        Data = EdytowanaTransakcja?.Data ?? DateTime.Now,
        Uzytkownik = login
    };
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
        Kwota = EdytowanaTransakcja != null ? -Math.Abs(kwota) : -Math.Abs(kwota); // zawsze negatywna kwota
        Kategoria = (KategoriaComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Inne";
        Opis = OpisTextBox.Text;

        this.Close(true);
    }
    else
    {
        Console.WriteLine("Uzupełnij wszystkie pola!");
        this.Close(false);
    }
}


        private void Anuluj_Click(object? sender, RoutedEventArgs e)
        {
            this.Close(false); // cancelled
        }
    
            private void Window_Loaded(object? sender, EventArgs e)
    {
        KwotaTextBox.Text = Kwota.ToString("N2");
        OpisTextBox.Text = Opis;
        KategoriaComboBox.SelectedItem = Kategoria;
        // ewentualnie inne pola
    }



    }
}
