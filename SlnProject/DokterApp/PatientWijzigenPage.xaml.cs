using Lib;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DokterApp
{
    /// <summary>
    /// Formulier waarmee de dokter de naam, contactgegevens en geboortedatum van een patiënt kan wijzigen.
    /// Geslacht, notificaties en wachtwoord worden niet getoond — die beheert de patiënt zelf.
    /// </summary>
    public partial class PatientWijzigenPage : Page
    {
        private int _patientId;
        private Patient _patient;

        public PatientWijzigenPage(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
        }

        // Laad bestaande gegevens in het formulier
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadPatient();
        }

        private void LaadPatient()
        {
            TxtFout.Text = string.Empty;

            try
            {
                _patient = Patient.GetById(_patientId);

                if (_patient == null)
                {
                    TxtFout.Text = "Patiënt niet gevonden.";
                    return;
                }

                TxtVoornaam.Text      = _patient.Voornaam;
                TxtAchternaam.Text    = _patient.Achternaam;
                TxtEmail.Text         = _patient.Email;
                TxtGsm.Text           = _patient.Gsm;
                TxtGeboortedatum.Text = _patient.Geboortedatum.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van de patiënt: " + ex.Message;
            }
        }

        // Formulier valideren en wijzigingen opslaan
        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (!ValideerFormulier())
            {
                return;
            }

            try
            {
                // enkel de toegestane velden overschrijven; overige waarden blijven ongewijzigd
                _patient.Voornaam      = TxtVoornaam.Text.Trim();
                _patient.Achternaam    = TxtAchternaam.Text.Trim();
                _patient.Email         = TxtEmail.Text.Trim();
                _patient.Gsm           = TxtGsm.Text.Trim();
                _patient.Geboortedatum = DateTime.ParseExact(
                    TxtGeboortedatum.Text.Trim(), "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);

                _patient.UpdateInDb();
                NavigationService.Navigate(new PatientDetailsPage(_patientId));
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het opslaan: " + ex.Message;
            }
        }

        // Annuleren: terug naar de detailpagina
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientDetailsPage(_patientId));
        }

        // Rechtstreeks naar het patiëntenoverzicht
        private void BtnNaarOverzicht_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientenOverzichtPage());
        }

        /// <summary>
        /// Valideert alle verplichte velden en het e-mailformaat.
        /// </summary>
        private bool ValideerFormulier()
        {
            TxtFout.Text = string.Empty;

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

            // e-mailformaat controleren: moet @ bevatten met domein erna
            string email = TxtEmail.Text.Trim();
            int atPos = email.IndexOf('@');
            if (atPos < 1 || atPos == email.Length - 1 || email.IndexOf('.', atPos) == -1)
            {
                TxtFout.Text = "Voer een geldig e-mailadres in (bv. naam@domein.be).";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtGeboortedatum.Text))
            {
                TxtFout.Text = "Geboortedatum is verplicht (formaat: dd/MM/yyyy).";
                return false;
            }

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
    }
}
