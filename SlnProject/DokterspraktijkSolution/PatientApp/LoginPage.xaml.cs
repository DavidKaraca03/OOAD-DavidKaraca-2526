using Lib;
using System.Windows;
using System.Windows.Controls;

namespace PatientApp
{
    /// <summary>
    /// Loginpagina voor de patiënt. Controleert e-mail en wachtwoord via de database.
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnInloggen_Click(object sender, RoutedEventArgs e)
        {
            TxtFout.Text = string.Empty;

            // Verplichte velden controleren
            if (string.IsNullOrWhiteSpace(TxtEmail.Text) || TxtPaswoord.Password.Length == 0)
            {
                TxtFout.Text = "Vul uw e-mail en wachtwoord in.";
                return;
            }

            Patient patient = Patient.Login(TxtEmail.Text.Trim(), TxtPaswoord.Password);

            if (patient == null)
            {
                TxtFout.Text = "Ongeldig e-mailadres of wachtwoord.";
                return;
            }

            // Inloggen geslaagd: navigatiepaneel tonen
            MainWindow.IngelogdePatient = patient;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ToonNavigatie();
        }
    }
}
