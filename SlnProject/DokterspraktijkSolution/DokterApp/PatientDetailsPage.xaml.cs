using Lib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DokterApp
{
    /// <summary>
    /// Toont alle details van één patiënt. Biedt knoppen voor wijzigen en verwijderen.
    /// </summary>
    public partial class PatientDetailsPage : Page
    {
        // Id van de te tonen patiënt, meegegeven via constructor
        private int _patientId;

        // Bijgehouden patiëntobject voor gebruik in de knoppen
        private Patient _patient;

        public PatientDetailsPage(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
        }

        // Laad patiëntgegevens bij het openen van de pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadPatient();
        }

        /// <summary>
        /// Haalt de patiënt op uit de database en vult alle velden in.
        /// </summary>
        private void LaadPatient()
        {
            TxtFout.Text = string.Empty;

            try
            {
                // patiënt ophalen via het meegegeven id
                _patient = Patient.GetById(_patientId);

                if (_patient == null)
                {
                    TxtFout.Text = "Patiënt niet gevonden.";
                    return;
                }

                // tekstvelden invullen
                LblVoornaam.Text      = _patient.Voornaam;
                LblAchternaam.Text    = _patient.Achternaam;
                LblEmail.Text         = _patient.Email;
                LblGsm.Text           = _patient.Gsm;
                LblGeslacht.Text      = _patient.Geslacht.ToString();
                LblGeboortedatum.Text = _patient.Geboortedatum.ToString("dd/MM/yyyy");
                LblNotificaties.Text  = _patient.Notificaties.ToString();

                // profielfoto tonen als aanwezig
                if (_patient.Profielfotodata != null)
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(_patient.Profielfotodata))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                    }
                    ImgFoto.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van de patiënt: " + ex.Message;
            }
        }

        // Navigeer naar de wijzigingspagina
        private void BtnWijzigen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientWijzigenPage(_patientId));
        }

        // Verwijder de patiënt na bevestiging via MessageBox
        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            if (_patient == null)
            {
                return;
            }

            // bevestiging vragen (MessageBox mag voor destructieve acties)
            MessageBoxResult keuze = MessageBox.Show(
                $"Weet je zeker dat je {_patient} wil verwijderen?\nAlle afspraken worden ook verwijderd.",
                "Bevestig verwijdering",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);

            if (keuze != MessageBoxResult.OK)
            {
                return;
            }

            try
            {
                // patiënt verwijderen uit de database (inclusief afspraken)
                _patient.DeleteFromDb();

                // terugkeren naar het overzicht
                NavigationService.Navigate(new PatientenOverzichtPage());
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het verwijderen: " + ex.Message;
            }
        }

        // Ga terug naar het patiëntenoverzicht
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientenOverzichtPage());
        }
    }
}
