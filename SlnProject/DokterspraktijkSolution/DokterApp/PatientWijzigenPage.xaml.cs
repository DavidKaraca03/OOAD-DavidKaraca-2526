using Lib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DokterApp
{
    /// <summary>
    /// Formulier om de gegevens van een bestaande patiënt te wijzigen.
    /// </summary>
    public partial class PatientWijzigenPage : Page
    {
        // Id van de te wijzigen patiënt, meegegeven via constructor
        private int _patientId;

        // Bijgehouden patiëntobject voor de update
        private Patient _patient;

        // Nieuwe fotobytes, enkel ingesteld als de gebruiker een nieuwe foto kiest
        private byte[] _nieuweFotoBytes;

        public PatientWijzigenPage(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
        }

        // Laad bestaande gegevens in het formulier bij het openen van de pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadPatient();
        }

        /// <summary>
        /// Haalt de patiënt op en vult alle formuliervelden in met de huidige waarden.
        /// </summary>
        private void LaadPatient()
        {
            TxtFout.Text = string.Empty;

            try
            {
                // patiënt ophalen uit de database
                _patient = Patient.GetById(_patientId);

                if (_patient == null)
                {
                    TxtFout.Text = "Patiënt niet gevonden.";
                    return;
                }

                // tekstvelden invullen
                TxtVoornaam.Text      = _patient.Voornaam;
                TxtAchternaam.Text    = _patient.Achternaam;
                TxtEmail.Text         = _patient.Email;
                TxtGsm.Text           = _patient.Gsm;
                TxtGeboortedatum.Text = _patient.Geboortedatum.ToString("dd/MM/yyyy");

                // keuzelijsten instellen op de huidige waarden
                CboGeslacht.SelectedIndex    = (int)_patient.Geslacht;
                CboNotificaties.SelectedIndex = (int)_patient.Notificaties;

                // huidige profielfoto tonen indien aanwezig
                if (_patient.Profielfotodata != null)
                {
                    ImgFoto.Source = BytesNaarBitmap(_patient.Profielfotodata);
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van de patiënt: " + ex.Message;
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

        // Formulier valideren, patiënt bijwerken en terugkeren naar details
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
                _patient.Geboortedatum = Convert.ToDateTime(TxtGeboortedatum.Text);
                _patient.Notificaties  = (NotificatieType)CboNotificaties.SelectedIndex;

                // nieuwe foto enkel overnemen als de gebruiker er één gekozen heeft
                if (_nieuweFotoBytes != null)
                {
                    _patient.Profielfotodata = _nieuweFotoBytes;
                }

                // opslaan in de database
                _patient.UpdateInDb();

                // terugkeren naar de detailpagina
                NavigationService.Navigate(new PatientDetailsPage(_patientId));
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het opslaan: " + ex.Message;
            }
        }

        // Annuleren: terugkeren naar de detailpagina zonder op te slaan
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientDetailsPage(_patientId));
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

            // geboortedatum geldigheid controleren
            try
            {
                Convert.ToDateTime(TxtGeboortedatum.Text);
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
