using Lib;
using System.Windows;

namespace PatientApp
{
    /// <summary>
    /// Hoofdvenster van de PatientApp. Bevat navigatiepaneel en hoofdframe.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Bijhouden welke patiënt momenteel is ingelogd
        public static Patient IngelogdePatient { get; set; }

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
            HoofdFrame.Navigate(new AfsprakenOverzichtPage());
        }

        // Navigeer naar het afsprakenoverzicht
        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            HoofdFrame.Navigate(new AfsprakenOverzichtPage());
        }

        // Navigeer naar het formulier voor een nieuwe afspraak
        private void BtnNieuweAfspraak_Click(object sender, RoutedEventArgs e)
        {
            HoofdFrame.Navigate(new NieuweAfspraakPage());
        }

        // Navigeer naar de profielpagina (wordt geïmplementeerd in dag 6)
        private void BtnProfiel_Click(object sender, RoutedEventArgs e)
        {
        }

        // Afmelden: verberg navigatie en keer terug naar loginpagina
        private void BtnAfmelden_Click(object sender, RoutedEventArgs e)
        {
            // ingelogde patiënt wissen
            IngelogdePatient = null;

            // navigatiepaneel verbergen en terug naar login
            NavPanel.Visibility = Visibility.Collapsed;
            HoofdFrame.Navigate(new LoginPage());
        }
    }
}
