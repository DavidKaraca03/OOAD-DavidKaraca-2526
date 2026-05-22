using Lib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PatientApp
{
    /// <summary>
    /// Toont de profielgegevens van de ingelogde patiënt in read-only weergave.
    /// </summary>
    public partial class ProfielInfoPage : Page
    {
        public ProfielInfoPage()
        {
            InitializeComponent();
        }

        // Laad de gegevens bij het openen van de pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadProfiel();
        }

        /// <summary>
        /// Haalt de meest recente gegevens op van de ingelogde patiënt en vult alle velden in.
        /// </summary>
        private void LaadProfiel()
        {
            TxtFout.Text = string.Empty;

            try
            {
                // actuele patiëntgegevens ophalen uit de database
                Patient patient = Patient.GetById(MainWindow.IngelogdePatient.Id);

                if (patient == null)
                {
                    TxtFout.Text = "Profielgegevens konden niet worden geladen.";
                    return;
                }

                // ingelogde patiënt bijwerken met verse gegevens
                MainWindow.IngelogdePatient = patient;

                // tekstvelden invullen
                LblVoornaam.Text      = patient.Voornaam;
                LblAchternaam.Text    = patient.Achternaam;
                LblEmail.Text         = patient.Email;
                LblGsm.Text           = patient.Gsm;
                LblGeslacht.Text      = patient.Geslacht.ToString();
                LblGeboortedatum.Text = patient.Geboortedatum.ToString("dd/MM/yyyy");
                LblNotificaties.Text  = patient.Notificaties.ToString();

                // profielfoto tonen als aanwezig
                if (patient.Profielfotodata != null)
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(patient.Profielfotodata))
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
                TxtFout.Text = "Fout bij het laden van het profiel: " + ex.Message;
            }
        }

        // Navigeer naar de wijzigingspagina
        private void BtnWijzigen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfielWijzigenPage());
        }
    }
}
