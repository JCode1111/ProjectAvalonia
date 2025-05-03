using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Project.Views;
using Project.Models;
using Project.Services;
using System.Linq;
using System;

namespace Project.Views
{
    public partial class MainWindow : Window
    {
        public static Uzytkownik? UzytkownikZalogowany { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
        }

        private async void Logowanie_Click(object? sender, RoutedEventArgs e)
{
    var dialog = new LogowanieWindow();
    // Użyj generic ShowDialog<bool> aby odebrać wynik Close(true/false)
    var result = await dialog.ShowDialog<bool>(this);

    if (result && MainWindow.UzytkownikZalogowany != null)
    {
        UpdateUI();
        WczytajTransakcjeIUstawSaldo(); // <- dodaj to
    }
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
            bool zalogowany = UzytkownikZalogowany != null;

            // Menu items visibility
            MenuLogowanie.IsVisible = !zalogowany;
            MenuRejestracja.IsVisible = !zalogowany;
            MenuWyloguj.IsVisible = zalogowany;

            // Panels visibility
            PanelNieZalogowany.IsVisible = !zalogowany;
            PanelZalogowany.IsVisible = zalogowany;

            if (UzytkownikZalogowany != null)
            {
                PowitanieTextBlock.Text = $"Zalogowany jako: {UzytkownikZalogowany.Login}";
                SaldoText.Text = $"Saldo: {UzytkownikZalogowany.Saldo} PLN";
                // Pokaż panel zalogowany...
            }
            else
            {
                // Clear history when logged out
                // HistoriaListBox.Items = new List<string>();
            }
        }
private async void DodajTransakcje_Click(object? sender, RoutedEventArgs e)
{
    if (UzytkownikZalogowany == null) return;

    var okno = new DodajTransakcjeWindow();
    var result = await okno.ShowDialog<bool>(this);

    if (!result) return;

    var t = new Transakcja
    {
        Data = DateTime.Now,
        Kwota = okno.Kwota,
        Kategoria = okno.Kategoria,
        Opis = okno.Opis,
        Uzytkownik = UzytkownikZalogowany.Login,
        ZalacznikSciezka = okno.ZalacznikSciezka
    };

    // 1. Zapis transakcji
    TransakcjaService.Zapisz(t);

    // 2. Przelicz saldo na podstawie wszystkich transakcji i zapisz do users.json
    UserService.PrzeliczISaveSaldo(UzytkownikZalogowany.Login);

    // 3. Wczytaj nowe saldo z transakcji
    UzytkownikZalogowany.Saldo = TransakcjaService.WczytajDlaUzytkownika(UzytkownikZalogowany.Login)
        .Sum(x => x.Kwota);

    // 4. Odśwież UI
    WczytajTransakcjeIUstawSaldo();
}


private void WczytajTransakcjeIUstawSaldo()
{
    if (UzytkownikZalogowany == null)
        return;

    // 1. Wczytaj wszystkie transakcje dla użytkownika
    var listaTransakcji = TransakcjaService.WczytajDlaUzytkownika(UzytkownikZalogowany.Login);

    // 2. Wyświetl w ListBox
    TransakcjeListBox.ItemsSource = listaTransakcji
        .Select(t => $"{t.Data:yyyy-MM-dd HH:mm} | {t.Kategoria,-10} | {t.Opis,-20} | {t.Kwota,8:N2} PLN")
        .ToList();

    // 3. Oblicz saldo jako sumę kwot
    decimal saldo = listaTransakcji.Sum(t => t.Kwota);

    // 4. Pokaz saldo w UI
    SaldoText.Text = $"Saldo: {saldo:N2} PLN";

    // 5. (Opcjonalnie) Zapisz je w users.json
    UzytkownikZalogowany.Saldo = saldo;
    UserService.AktualizujSaldo(UzytkownikZalogowany.Login, saldo);
}


        public void AktualizujWidokDlaZalogowanego()
        {
            WczytajTransakcjeIUstawSaldo();
        }
    }
}
