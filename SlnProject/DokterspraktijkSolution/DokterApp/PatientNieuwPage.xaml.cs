using Lib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DokterApp
{
    /// <summary>
    /// Formulier om een nieuwe patiënt aan te maken in de database.
    /// </summary>
    public partial class PatientNieuwPage : Page
    {
        // Fotobytes van de gekozen profielfoto, null als geen foto gekozen
        private byte[] _fotoBytes;

        public PatientNieuwPage()
        {
            InitializeComponent();
        }

        // Standaard selecties instellen bij het openen van de pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CboGeslacht.SelectedIndex    = 0;
            CboNotificaties.SelectedIndex = 0;
        }

        // Bestandsdialoog openen om een profielfoto te kiezen
        private void BtnFotoKiezen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialoog = new Microsoft.Win32.OpenFileDialog();
            dialoog.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png;*.bmp";
            dialoog.Title  = "Kies een profielfoto";

            if (dialoog.ShowDialog() == true)
            {
                // afbeelding inlezen als bytes en preview tonen
                _fotoBytes     = File.ReadAllBytes(dialoog.FileName);
                ImgFoto.Source = BytesNaarBitmap(_fotoBytes);
            }
        }

        // Formulier valideren, patiënt aanmaken en navigeren naar de detailpagina
        private void BtnAanmaken_Click(object sender, RoutedEventArgs e)
        {
            if (!ValideerFormulier())
            {
                return;
            }

            try
            {
                // nieuw patiëntobject opbouwen
                Patient nieuwPatient = new Patient();
                nieuwPatient.Voornaam        = TxtVoornaam.Text.Trim();
                nieuwPatient.Achternaam      = TxtAchternaam.Text.Trim();
                nieuwPatient.Email           = TxtEmail.Text.Trim();
                nieuwPatient.Gsm             = TxtGsm.Text.Trim();
                nieuwPatient.Geslacht        = (GeslachtType)CboGeslacht.SelectedIndex;
                nieuwPatient.Geboortedatum   = Convert.ToDateTime(TxtGeboortedatum.Text);
                nieuwPatient.Notificaties    = (NotificatieType)CboNotificaties.SelectedIndex;
                nieuwPatient.Profielfotodata = _fotoBytes;

                // wachtwoord hashen voor opslag
                nieuwPatient.Paswoord = Persoon.HashPaswoord(TxtPaswoord.Password);

                // patiënt invoegen in de database en nieuw id ophalen
                int nieuwId = nieuwPatient.InsertInDb();

                // navigeren naar de detailpagina van de zojuist aangemaakte patiënt
                NavigationService.Navigate(new PatientDetailsPage(nieuwId));
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het aanmaken van de patiënt: " + ex.Message;
            }
        }

        // Annuleren: terugkeren naar het patiëntenoverzicht
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientenOverzichtPage());
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

            // wachtwoord controleren
            if (TxtPaswoord.Password.Length == 0)
            {
                TxtFout.Text = "Wachtwoord is verplicht.";
                return false;
            }
            if (TxtPaswoord.Password != TxtPaswoordBevestig.Password)
            {
                TxtFout.Text = "De wachtwoorden komen niet overeen.";
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
