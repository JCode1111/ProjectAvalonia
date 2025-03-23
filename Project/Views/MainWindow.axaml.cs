using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Project.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();  // Inicjalizuje kontrolki z XAML
            this.WindowState = WindowState.Maximized;  // Ustawia okno na pełny ekran
        }

        // Obsługuje kliknięcie w "Witaj użytkowniku"
        private void WitajUzytkownikuMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new TextBlock
            {
                Text = "Witaj użytkowniku!",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }

        // Obsługuje kliknięcie w "Saldo"
        private void SaldoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new TextBlock
            {
                Text = "Twoje saldo wynosi: 5000 PLN",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }
    }
}
