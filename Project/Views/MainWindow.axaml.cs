using Avalonia.Controls;
using Avalonia.Interactivity;
using Project.Views;

namespace Project.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }

        private void DodajTransakcje_Click(object sender, RoutedEventArgs e)
        {
            var popup = new DodajTransakcjeWindow();
            popup.ShowDialog(this); // Wyświetlenie jako modalnego okna
        }
    }
}
