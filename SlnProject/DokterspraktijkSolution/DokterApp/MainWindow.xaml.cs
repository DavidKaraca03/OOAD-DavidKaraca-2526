using Lib;
using System.Windows;

namespace DokterApp
{
    /// <summary>
    /// Hoofdvenster van de DokterApp. Bevat navigatiepaneel en hoofdframe.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Bijhouden welke dokter momenteel is ingelogd
        public static Dokter IngelogdeDokter { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        // Navigeer naar de loginpagina bij het openen van het venster
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HoofdFrame.Navigate(new LoginPage());
        }

        // Maakt het navigatiepaneel zichtbaar na succesvolle login
        public void ToonNavigatie()
        {
            NavPanel.Visibility = Visibility.Visible;
        }
    }
}
