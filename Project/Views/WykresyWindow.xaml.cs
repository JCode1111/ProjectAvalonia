using Avalonia.Controls;
using LiveChartsCore; 
using LiveChartsCore.SkiaSharpView; 
using LiveChartsCore.SkiaSharpView.Avalonia; 
using LiveChartsCore.Measure; 
using Project.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Project.Services;

namespace Project.Views
{
    // okno wykresów
    public partial class WykresyWindow : Window
    {
        public string Naglowek { get; set; }
        public ObservableCollection<ISeries> KategoriaSeries { get; set; } = new();
        public ObservableCollection<ISeries> MiesieczneWydatkiSeries { get; set; } = new();
        public Axis[] XOsMiesieczne { get; set; }
        public Axis[] YOsKwoty { get; set; }


        public WykresyWindow(string login)
        {
            InitializeComponent();

            Naglowek = $"Zalogowany jako: {login}";

            var transakcje = TransakcjaService.WczytajDlaUzytkownika(login);

            var tylkoWydatki = transakcje.Where(t => t.Kwota < 0).ToList();

            // Wydatki wg kategorii (kołowy wykres)
            var kategorie = tylkoWydatki
                .GroupBy(t => t.Kategoria)
                .Select(g => new PieSeries<double>
                {
                    Name = g.Key,
                    Values = new[] { (double)(-g.Sum(t => t.Kwota)) } // zamieniamy na dodatnie
                });

            KategoriaSeries = new ObservableCollection<ISeries>(kategorie);

            // Wydatki miesięczne (słupkowy wykres)
            var miesieczne = tylkoWydatki
                .GroupBy(t => t.Data.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .ToList();

            var etykiety = miesieczne.Select(g => g.Key).ToArray();
            var kwoty = miesieczne.Select(g => (double)(-g.Sum(t => t.Kwota))).ToArray();

            MiesieczneWydatkiSeries = new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Name = "Wydatki miesięczne",
                    Values = kwoty
                }
            };

            XOsMiesieczne = new[]
            {
                new Axis { Labels = etykiety }
            };

            YOsKwoty = new[]
            {
                new Axis { Name = "Kwota (PLN)" }
            };

            DataContext = this;
        }

        // odswiezanie wykresów 
        public void OdswiezWykresy(string login)
        {
            var transakcje = TransakcjaService.WczytajDlaUzytkownika(login);

            var tylkoWydatki = transakcje.Where(t => t.Kwota < 0).ToList();

            // kolowy wykres
            KategoriaSeries.Clear();
            foreach (var seria in tylkoWydatki
                .GroupBy(t => t.Kategoria)
                .Select(g => new PieSeries<double>
                {
                    Name = g.Key,
                    Values = new[] { (double)(-g.Sum(t => t.Kwota)) }
                }))
            {
                KategoriaSeries.Add(seria);
            }

            // slupkowy wykres
            var miesieczne = tylkoWydatki
                .GroupBy(t => t.Data.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .ToList();

            var etykiety = miesieczne.Select(g => g.Key).ToArray();
            var kwoty = miesieczne.Select(g => (double)(-g.Sum(t => t.Kwota))).ToArray();

            MiesieczneWydatkiSeries.Clear();
            MiesieczneWydatkiSeries.Add(new ColumnSeries<double>
            {
                Name = "Wydatki miesięczne",
                Values = kwoty
            });

            XOsMiesieczne = new[]
            {
                new Axis { Labels = etykiety }
            };

            YOsKwoty = new[]
            {
                new Axis { Name = "Kwota (PLN)" }
            };

            DataContext = null;
            DataContext = this;
        }
    }
}