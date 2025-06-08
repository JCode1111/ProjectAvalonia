using Avalonia.Controls;
using Avalonia.VisualTree;
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
using Avalonia.Input;



namespace Project.Views
{
    public partial class MainWindow : Window
    {
        public static Uzytkownik? UzytkownikZalogowany { get; set; }
        private string? WybranaKategoria = null;
        private DateTime? DataOd = null;
        private DateTime? DataDo = null;
        private List<Transakcja> _aktualneTransakcje = new();
        private string? WybraneSortowanie = null;

        private WykresyWindow? _wykresyWindow;


        private DateTime _ostatnieKlikniecie = DateTime.MinValue;
        private TransakcjaWidok? _ostatnioKliknieta = null;

        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
        }

        //Logowanie
        private async void Logowanie_Click(object? sender, RoutedEventArgs e)
        {
            var dialog = new LogowanieWindow();

            var result = await dialog.ShowDialog<bool>(this);

            if (result && MainWindow.UzytkownikZalogowany != null)
            {
                UpdateUI();
                WczytajTransakcjeIUstawSaldo(); 
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

        //otwarcie wykresów uzytkownika
        private void OtworzWykresy_Click(object? sender, RoutedEventArgs e)
        {
            if (_wykresyWindow == null || !_wykresyWindow.IsVisible)
            {
                _wykresyWindow = new WykresyWindow(UzytkownikZalogowany.Login);
                _wykresyWindow.Closed += (s, args) => _wykresyWindow = null;
                _wykresyWindow.Show();
            }
            else
            {
                _wykresyWindow.Activate();
            }
        }


        //Aktualizacja UI
        private void UpdateUI()
        {
            bool zalogowany = UzytkownikZalogowany != null;

            MenuLogowanie.IsVisible = !zalogowany;
            MenuRejestracja.IsVisible = !zalogowany;
            MenuWyloguj.IsVisible = zalogowany;
            MenuWykresy.IsVisible = zalogowany;


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

            // Zapis transakcji
            TransakcjaService.Zapisz(t);

            // Przeliczanie salda i zapis
            UserService.PrzeliczISaveSaldo(UzytkownikZalogowany.Login);

            // Odświeżanie salda 
            UzytkownikZalogowany.Saldo = TransakcjaService
                .WczytajDlaUzytkownika(UzytkownikZalogowany.Login)
                .Sum(x => x.Kwota);

            // Sprawdzanie wszystkiich limitów
            SprawdzWszystkieLimityIWyświetl();

            //Odświeżanie UI
            WczytajTransakcjeIUstawSaldo();
    
            _wykresyWindow?.OdswiezWykresy(UzytkownikZalogowany.Login);

        }

        // obsługa usuwania transakcji
        private void UsunTransakcje_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TransakcjaWidok widok)
            {
                TransakcjaService.Usun(widok.OryginalnaTransakcja);
                WczytajTransakcjeIUstawSaldo();
            }
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
                    Kwota = wypłataWindow.Kwota,
                    Kategoria = "Wypłata",
                    Opis = wypłataWindow.Opis,
                    Uzytkownik = UzytkownikZalogowany.Login
                };

                TransakcjaService.Zapisz(transakcja);
                UserService.PrzeliczISaveSaldo(UzytkownikZalogowany.Login);
                WczytajTransakcjeIUstawSaldo();
                _wykresyWindow?.OdswiezWykresy(UzytkownikZalogowany.Login);
            }
        }

        //Filtry 
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

        // zaczytanie transakcji uzytkownika
        private void WczytajTransakcjeIUstawSaldo()
        {
            if (UzytkownikZalogowany == null) return;

            var wszystkie = TransakcjaService.WczytajDlaUzytkownika(UzytkownikZalogowany.Login);

            var saldo = wszystkie.Sum(t => t.Kwota);
            SaldoText.Text = $"Saldo: {saldo:N2} PLN";

            UzytkownikZalogowany.Saldo = saldo;
            UserService.AktualizujSaldo(UzytkownikZalogowany.Login, saldo);

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

            _aktualneTransakcje = filtrowane.ToList();

            var listaWidokowa = _aktualneTransakcje
                .Select((t, i) => new TransakcjaWidok
                {
                    Index = i,
                    Wyswietlacz = $"{t.Data:yyyy-MM-dd HH:mm} | {t.Kategoria,-10} | {t.Opis,-20} | {t.Kwota,8:N2} PLN",
                    OryginalnaTransakcja = t
                })
                .ToList();

            TransakcjeListBox.ItemsSource = listaWidokowa;

            SprawdzWszystkieLimityIWyświetl();
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

            SprawdzWszystkieLimityIWyświetl();
        }

        //Sprawdzanie limitów uzytkownika
        private void SprawdzWszystkieLimityIWyświetl()
        {
            var user = UserService.Wczytaj(UzytkownikZalogowany.Login);
            if (user == null) return;

            var transakcjeUzytkownika = TransakcjaService.WczytajDlaUzytkownika(user.Login)
                .Where(t => t.Data.Month == DateTime.Now.Month && t.Data.Year == DateTime.Now.Year)
                .ToList();

            var komunikaty = new List<string>();

            foreach (var limit in user.LimityBudzetowe)
            {
                var transakcje = transakcjeUzytkownika.Where(t => t.Kategoria == limit.Kategoria);
                var suma = transakcje.Sum(t => Math.Abs(t.Kwota));
                var procent = suma / limit.Limit;

                if (procent >= 1.0m)
                    komunikaty.Add($"UWAGA Przekroczono limit: '{limit.Kategoria}' ({suma} / {limit.Limit} zł)");
                else if (procent >= 0.8m)
                    komunikaty.Add($"UWAGA: Zbliżasz się do limitu: '{limit.Kategoria}' ({suma} / {limit.Limit} zł)");
            }

            if (komunikaty.Any())
            {
                LimitInfoTextBlock.Text = string.Join("\n", komunikaty);
                LimitInfoTextBlock.Foreground = Brushes.Orange;
                LimitInfoTextBlock.TextAlignment = TextAlignment.Center;
            }
            else
            {
                LimitInfoTextBlock.Text = "";
            }
        }


        // Obsługa Zdarzenia doubleclick - do zmiany transakcji
        private async void TransakcjaWidok_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is StackPanel panel && panel.DataContext is TransakcjaWidok widok)
            {
                var teraz = DateTime.Now;

                if ((teraz - _ostatnieKlikniecie).TotalMilliseconds < 500 && _ostatnioKliknieta == widok)
                {
                    var okno = new DodajTransakcjeWindow(widok.OryginalnaTransakcja);
                    var rezultat = await okno.ShowDialog<bool>(this);

                    if (rezultat)
                    {
                        TransakcjaService.Zamien(widok.OryginalnaTransakcja,
                            okno.ZwrocTransakcje(UzytkownikZalogowany.Login));
                        WczytajTransakcjeIUstawSaldo();
                    }
                }

                _ostatnieKlikniecie = teraz;
                _ostatnioKliknieta = widok;
            }
        }



        public void AktualizujWidokDlaZalogowanego()
        {
            WczytajTransakcjeIUstawSaldo();
        }
    }
}
