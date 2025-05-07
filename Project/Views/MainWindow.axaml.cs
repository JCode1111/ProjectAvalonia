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
using Avalonia.Media;

namespace Project.Views
{
    public partial class MainWindow : Window
    {
        public static Uzytkownik? UzytkownikZalogowany { get; set; }
        private string? WybranaKategoria = null;
        private DateTime? DataOd = null;
        private DateTime? DataDo = null;

        private string? WybraneSortowanie = null;

        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
        }

        //Logowanie
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


        //Rejestracja
        private void Rejestracja_Click(object? sender, RoutedEventArgs e)
        {
            var regWindow = new RejestracjaWindow();
            regWindow.ShowDialog(this);
            UpdateUI();
        }

        //Wylogowanie        
        private void Wyloguj_Click(object? sender, RoutedEventArgs e)
        {
            UzytkownikZalogowany = null;
            UpdateUI();
        }


        //Aktualizacja UI
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
                
                var transakcje = TransakcjaService.WczytajDlaUzytkownika(UzytkownikZalogowany.Login);
                var kategorie = transakcje.Select(t=>t.Kategoria).Distinct().OrderBy(x=> x).ToList();
                KategorieListBox.ItemsSource = kategorie;

                WczytajTransakcjeIUstawSaldo();

            }
            else
            {
                // Clear history when logged out
                // HistoriaListBox.Items = new List<string>();
            }
        }

        //Dodawanie transakcji
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

            var komunikat = SprawdzLimitZuzycia(t.Kategoria);
            if (!string.IsNullOrEmpty(komunikat))
            {
                LimitInfoTextBlock.Text = komunikat;
                LimitInfoTextBlock.Foreground = Brushes.Orange;
                LimitInfoTextBlock.TextAlignment = TextAlignment.Center;
            }

            // 4. Odśwież UI
            WczytajTransakcjeIUstawSaldo();
        }

        //Dodawanie wyplaty
        private async void DodajWyplate_Click(object? sender, RoutedEventArgs e)
        {
            var wypłataWindow = new DodajWyplateWindow();
            var result = await wypłataWindow.ShowDialog<bool>(this);

            if (result)
            {
                var transakcja = new Transakcja
                {
                    Data = DateTime.Now,
                    Kwota = wypłataWindow.Kwota, // wartość dodatnia
                    Kategoria = "Wypłata",
                    Opis = wypłataWindow.Opis,
                    Uzytkownik = UzytkownikZalogowany.Login
                };

                TransakcjaService.Zapisz(transakcja);
                UserService.PrzeliczISaveSaldo(UzytkownikZalogowany.Login);
                WczytajTransakcjeIUstawSaldo(); // odśwież dane
            }
        }

        private void Kategorie_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            WybranaKategoria = KategorieListBox.SelectedItem?.ToString();
            WczytajTransakcjeIUstawSaldo();
        }

        private void DataPicker_Changed(object? sender, DatePickerSelectedValueChangedEventArgs e)
        {
            DataOd = DataOdPicker.SelectedDate?.DateTime;
            DataDo = DataDoPicker.SelectedDate?.DateTime;
            WczytajTransakcjeIUstawSaldo();
        }

        private void SortowanieComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (SortowanieComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            WybraneSortowanie = selectedItem;
            WczytajTransakcjeIUstawSaldo();
        }

        // Wyczysc filtry
        private void WyczyscFiltry_Click(object? sender, RoutedEventArgs e)
        {
            WybranaKategoria = null;
            DataOdPicker.SelectedDate = null;
            DataDoPicker.SelectedDate = null;
            DataOd = null;
            DataDo = null;
            KategorieListBox.SelectedItem = null;
            SortowanieComboBox.SelectedItem = null;
            WybraneSortowanie = null;
            WczytajTransakcjeIUstawSaldo();
        }


        private void WczytajTransakcjeIUstawSaldo()
        {
            if (UzytkownikZalogowany == null) return;

            var wszystkie = TransakcjaService.WczytajDlaUzytkownika(UzytkownikZalogowany.Login);

                // Stałe saldo — pełna suma
            var saldo = wszystkie.Sum(t => t.Kwota);
            SaldoText.Text = $"Saldo: {saldo:N2} PLN";

            UzytkownikZalogowany.Saldo = saldo;
            UserService.AktualizujSaldo(UzytkownikZalogowany.Login, saldo);

        // Filtrowanie transakcji (jeśli aktywne)
            var filtrowane = wszystkie.AsEnumerable();

            if (!string.IsNullOrEmpty(WybranaKategoria))
                filtrowane = filtrowane.Where(t => t.Kategoria == WybranaKategoria);

            if (DataOd != null)
                filtrowane = filtrowane.Where(t => t.Data >= DataOd);

            if (DataDo != null)
                filtrowane = filtrowane.Where(t => t.Data <= DataDo);

            

            if (!string.IsNullOrEmpty(WybraneSortowanie))
            {
                filtrowane = WybraneSortowanie switch
                {
                    "Data rosnąco" => filtrowane.OrderBy(t => t.Data),
                    "Data malejąco" => filtrowane.OrderByDescending(t => t.Data),
                    "Kwota rosnąco" => filtrowane.OrderBy(t => t.Kwota),
                    "Kwota malejąco" => filtrowane.OrderByDescending(t => t.Kwota),
                    _ => filtrowane
                };
            }

            TransakcjeListBox.ItemsSource = filtrowane
            .Select(t => $"{t.Data:yyyy-MM-dd HH:mm} | {t.Kategoria,-10} | {t.Opis,-20} | {t.Kwota,8:N2} PLN")
            .ToList();
            
            // Sprawdzanie przekroczeń limitów po zalogowaniu/odświeżeniu
            var user = UserService.Wczytaj(UzytkownikZalogowany.Login);
            if (user != null && user.LimityBudzetowe != null)
            {
                foreach (var limit in user.LimityBudzetowe)
                {
                    var komunikat = SprawdzLimitZuzycia(limit.Kategoria);
                    if (!string.IsNullOrEmpty(komunikat))
                    {
                        LimitInfoTextBlock.Text = komunikat;
                        LimitInfoTextBlock.Foreground = Brushes.Orange;
                        break; // jeśli chcesz tylko 1 komunikat naraz
                    }
                }
            }

        }

        // Limity budżetowe dla danej kategorii
        private void ZapiszLimit_Click(object? sender, RoutedEventArgs e)
        {
            var kategoria = (KategoriaComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!decimal.TryParse(LimitTextBox.Text, out var limit) || string.IsNullOrWhiteSpace(kategoria))
            {
                LimitInfoTextBlock.Text = " Wprowadź poprawny limit i wybierz kategorię.";
                LimitInfoTextBlock.Foreground = Brushes.Red;
                return;
            }

            var user = UserService.Wczytaj(UzytkownikZalogowany.Login);
            if (user == null)
            {
                LimitInfoTextBlock.Text = " Błąd podczas wczytywania użytkownika.";
                LimitInfoTextBlock.Foreground = Brushes.Red;
                return;
            }

            var istniejący = user.LimityBudzetowe.FirstOrDefault(l => l.Kategoria == kategoria);
            if (istniejący != null)
                istniejący.Limit = limit;
            else
                user.LimityBudzetowe.Add(new LimitBudzetowy { Kategoria = kategoria, Limit = limit });

            UserService.Zapisz(user);

            LimitInfoTextBlock.Text = $" Limit zapisany dla kategorii {kategoria}.";
            LimitInfoTextBlock.Foreground = Brushes.Green;
        }

        private string? SprawdzLimitZuzycia(string kategoria)
        {
            var user = UserService.Wczytaj(UzytkownikZalogowany.Login);
            if (user == null) return null;

            var limit = user.LimityBudzetowe.FirstOrDefault(l => l.Kategoria == kategoria);
            if (limit == null) return null;

            var transakcje = TransakcjaService.WczytajDlaUzytkownika(user.Login)
                .Where(t => t.Kategoria == kategoria
                    && t.Data.Month == DateTime.Now.Month
                    && t.Data.Year == DateTime.Now.Year);

            var suma = transakcje.Sum(t => Math.Abs(t.Kwota)); 

            var procent = suma / limit.Limit;
            if (procent >= 1.0m)
                return $"Przekroczono limit dla kategorii '{kategoria}' ({suma} / {limit.Limit} zł)";
            else if (procent >= 0.8m)
                return $"Zbliżasz się do limitu dla kategorii '{kategoria}' ({suma} / {limit.Limit} zł)";

            return null;
        }



        public void AktualizujWidokDlaZalogowanego()
        {
            WczytajTransakcjeIUstawSaldo();
        }
    }
}
