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

        /// <summary>
        /// Maakt het navigatiepaneel zichtbaar en navigeert naar de afsprakenpagina.
        /// Wordt opgeroepen vanuit LoginPage na succesvolle login.
        /// </summary>
        public void ToonNavigatie()
        {
            NavPanel.Visibility = Visibility.Visible;
            HoofdFrame.Navigate(new AfsprakenPage());
        }

        // Navigeer naar de afsprakenpagina
        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            HoofdFrame.Navigate(new AfsprakenPage());
        }

        // Navigeer naar het patiëntenoverzicht
        private void BtnPatienten_Click(object sender, RoutedEventArgs e)
        {
            HoofdFrame.Navigate(new PatientenOverzichtPage());
        }

        // Afmelden: verberg navigatie en keer terug naar loginpagina
        private void BtnAfmelden_Click(object sender, RoutedEventArgs e)
        {
            // ingelogde dokter wissen
            IngelogdeDokter = null;

            // navigatiepaneel verbergen en terug naar login
            NavPanel.Visibility = Visibility.Collapsed;
            HoofdFrame.Navigate(new LoginPage());
        }
    }
}
