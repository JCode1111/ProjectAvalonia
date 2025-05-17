using Avalonia.Controls;
using LiveChartsCore; // ISeries
using LiveChartsCore.SkiaSharpView; // PieSeries, ColumnSeries
using LiveChartsCore.SkiaSharpView.Avalonia; // do integracji z Avalonia
using LiveChartsCore.Measure; // ewentualnie dla typów pomocniczych jak AxisOrientation
using Project.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Project.Services;

namespace Project.Views
{

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

    // 🔵 Tylko wydatki (ujemne kwoty)
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

public void OdswiezWykresy(string login)
{
    var transakcje = TransakcjaService.WczytajDlaUzytkownika(login);

    var tylkoWydatki = transakcje.Where(t => t.Kwota < 0).ToList();

    // KOŁOWY
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

    // SŁUPKOWY
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

//    public void OdswiezWykresy(string login)
// {
//     var transakcje = TransakcjaService.WczytajDlaUzytkownika(login);

//     // KOŁOWY
//     KategoriaSeries.Clear();
//     foreach (var seria in transakcje
//         .GroupBy(t => t.Kategoria)
//         .Select(g => new PieSeries<double>
//         {
//             Name = g.Key,
//             Values = new[] { (double)Math.Abs(g.Sum(t => t.Kwota)) }
//         }))
//     {
//         KategoriaSeries.Add(seria);
//     }

//     // SŁUPKOWY – miesięczne wydatki użytkownika
//     var miesieczne = transakcje
//         .GroupBy(t => t.Data.ToString("yyyy-MM"))
//         .OrderBy(g => g.Key)
//         .ToList();

//     var etykiety = miesieczne.Select(g => g.Key).ToArray();
//     var kwoty = miesieczne.Select(g => (double)Math.Abs(g.Sum(t => t.Kwota))).ToArray();

//     MiesieczneWydatkiSeries.Clear();
//     MiesieczneWydatkiSeries.Add(new ColumnSeries<double>
//     {
//         Name = "Wydatki miesięczne",
//         Values = kwoty
//     });

//     XOsMiesieczne = new[]
//     {
//         new Axis { Labels = etykiety }
//     };

//     YOsKwoty = new[]
//     {
//         new Axis { Name = "Kwota (PLN)" }
//     };

//     // Trigger re-binding
//     DataContext = null;
//     DataContext = this;
// }