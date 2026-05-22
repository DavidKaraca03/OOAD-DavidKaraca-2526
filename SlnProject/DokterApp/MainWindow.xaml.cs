using Lib;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

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

        // Navigeer naar de startpagina bij het openen van het venster
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HoofdFrame.Navigate(new StartPage());
        }

        /// <summary>
        /// Maakt het navigatiepaneel zichtbaar en navigeert naar de afsprakenpagina.
        /// Wordt opgeroepen vanuit LoginPage na succesvolle login.
        /// </summary>
        public void ToonNavigatie()
        {
            // naam en initiaal van de ingelogde dokter tonen
            LblGebruikerNaam.Text    = IngelogdeDokter.Voornaam + " " + IngelogdeDokter.Achternaam;
            LblGebruikerInitiaal.Text = IngelogdeDokter.Voornaam.Substring(0, 1).ToUpper();

            // profielfoto tonen indien aanwezig, anders initiaal als placeholder
            if (IngelogdeDokter.Profielfotodata != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(IngelogdeDokter.Profielfotodata))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                }
                ImgGebruikerFoto.Source     = bitmap;
                ImgGebruikerFoto.Visibility = Visibility.Visible;
            }

            // navigatiepaneel en topbalk zichtbaar maken
            TopBar.Visibility = Visibility.Visible;
            NavPanel.Visibility = Visibility.Visible;
            HoofdFrame.Navigate(new PatientenOverzichtPage());
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

            // navigatiepaneel en topbalk verbergen en terug naar login
            TopBar.Visibility           = Visibility.Collapsed;
            NavPanel.Visibility         = Visibility.Collapsed;
            ImgGebruikerFoto.Source     = null;
            ImgGebruikerFoto.Visibility = Visibility.Collapsed;
            HoofdFrame.Navigate(new LoginPage());
        }
    }
}
