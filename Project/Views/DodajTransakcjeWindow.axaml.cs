using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Project.Views
{
    public partial class DodajTransakcjeWindow : Window
    {
        public DodajTransakcjeWindow()
        {
            InitializeComponent(); // ✅ Naprawione wywołanie inicjalizacji XAML
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            if (KwotaTextBox != null) // ✅ Sprawdzenie, czy pole istnieje
            {
                string? kwota = KwotaTextBox?.Text; 
                // Tutaj można dodać logikę zapisu transakcji
            }
            this.Close();
        }
    }
}
