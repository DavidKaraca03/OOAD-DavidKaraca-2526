using Lib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PatientApp
{
    /// <summary>
    /// Formulier waarmee de ingelogde patiënt zijn eigen profielgegevens kan wijzigen.
    /// </summary>
    public partial class ProfielWijzigenPage : Page
    {
        // Bijgehouden patiëntobject voor de update
        private Patient _patient;

        // Nieuwe fotobytes, enkel ingesteld als de gebruiker een nieuwe foto kiest
        private byte[] _nieuweFotoBytes;

        public ProfielWijzigenPage()
        {
            InitializeComponent();
        }

        // Laad de gegevens bij het openen van de pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadProfiel();
        }

        /// <summary>
        /// Haalt de actuele patiëntgegevens op en vult alle formuliervelden in.
        /// </summary>
        private void LaadProfiel()
        {
            TxtFout.Text = string.Empty;

            try
            {
                // actuele gegevens ophalen uit de database
                _patient = Patient.GetById(MainWindow.IngelogdePatient.Id);

                if (_patient == null)
                {
                    TxtFout.Text = "Profielgegevens konden niet worden geladen.";
                    return;
                }

                // tekstvelden invullen met de huidige waarden
                TxtVoornaam.Text      = _patient.Voornaam;
                TxtAchternaam.Text    = _patient.Achternaam;
                TxtEmail.Text         = _patient.Email;
                TxtGsm.Text           = _patient.Gsm;
                TxtGeboortedatum.Text = _patient.Geboortedatum.ToString("dd/MM/yyyy");

                // keuzelijsten instellen op de huidige waarden
                CboGeslacht.SelectedIndex     = (int)_patient.Geslacht;
                CboNotificaties.SelectedIndex = (int)_patient.Notificaties;

                // huidige profielfoto tonen indien aanwezig
                if (_patient.Profielfotodata != null)
                {
                    ImgFoto.Source = BytesNaarBitmap(_patient.Profielfotodata);
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van het profiel: " + ex.Message;
            }
        }

        // Bestandsdialoog openen om een nieuwe profielfoto te kiezen
        private void BtnFotoKiezen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialoog = new Microsoft.Win32.OpenFileDialog();
            dialoog.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png;*.bmp";
            dialoog.Title  = "Kies een profielfoto";

            if (dialoog.ShowDialog() == true)
            {
                // afbeelding inlezen als bytes en preview tonen
                _nieuweFotoBytes = File.ReadAllBytes(dialoog.FileName);
                ImgFoto.Source   = BytesNaarBitmap(_nieuweFotoBytes);
            }
        }

        // Formulier valideren, gegevens opslaan en terugkeren naar profieloverzicht
        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (!ValideerFormulier())
            {
                return;
            }

            try
            {
                // gewijzigde waarden in het patiëntobject schrijven
                _patient.Voornaam      = TxtVoornaam.Text.Trim();
                _patient.Achternaam    = TxtAchternaam.Text.Trim();
                _patient.Email         = TxtEmail.Text.Trim();
                _patient.Gsm           = TxtGsm.Text.Trim();
                _patient.Geslacht      = (GeslachtType)CboGeslacht.SelectedIndex;
                _patient.Geboortedatum = DateTime.ParseExact(TxtGeboortedatum.Text.Trim(), "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);
                _patient.Notificaties  = (NotificatieType)CboNotificaties.SelectedIndex;

                // nieuwe foto enkel overnemen als de gebruiker er één gekozen heeft
                if (_nieuweFotoBytes != null)
                {
                    _patient.Profielfotodata = _nieuweFotoBytes;
                }

                // opslaan in de database
                _patient.UpdateInDb();

                // ingelogde patiënt bijwerken met de nieuwe gegevens
                MainWindow.IngelogdePatient = _patient;

                // terugkeren naar de profielpagina
                NavigationService.Navigate(new ProfielInfoPage());
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het opslaan: " + ex.Message;
            }
        }

        // Annuleren: terugkeren naar de profielpagina zonder op te slaan
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfielInfoPage());
        }

        /// <summary>
        /// Controleert of alle verplichte velden correct ingevuld zijn.
        /// </summary>
        private bool ValideerFormulier()
        {
            TxtFout.Text = string.Empty;

            // verplichte tekstvelden controleren
            if (string.IsNullOrWhiteSpace(TxtVoornaam.Text))
            {
                TxtFout.Text = "Voornaam is verplicht.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtAchternaam.Text))
            {
                TxtFout.Text = "Achternaam is verplicht.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                TxtFout.Text = "E-mailadres is verplicht.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtGeboortedatum.Text))
            {
                TxtFout.Text = "Geboortedatum is verplicht (formaat: dd/MM/yyyy).";
                return false;
            }

            // geboortedatum geldigheid controleren (expliciet dd/MM/yyyy formaat)
            try
            {
                DateTime.ParseExact(TxtGeboortedatum.Text.Trim(), "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                TxtFout.Text = "Geboortedatum is ongeldig. Gebruik het formaat dd/MM/yyyy.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Zet een byte-array om naar een BitmapImage voor gebruik in een Image-control.
        /// </summary>
        private BitmapImage BytesNaarBitmap(byte[] bytes)
        {
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                bitmap.BeginInit();
                bitmap.CacheOption  = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
            }
            return bitmap;
        }
    }
}
