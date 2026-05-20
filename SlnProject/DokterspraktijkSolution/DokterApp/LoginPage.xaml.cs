using Lib;
using System.Windows;
using System.Windows.Controls;

namespace DokterApp
{
    /// <summary>
    /// Loginpagina voor de dokter. Controleert e-mail en wachtwoord via de database.
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

            Dokter dokter = Dokter.Login(TxtEmail.Text.Trim(), TxtPaswoord.Password);

            if (dokter == null)
            {
                TxtFout.Text = "Ongeldig e-mailadres of wachtwoord.";
                return;
            }

            // Inloggen geslaagd: navigatiepaneel tonen
            MainWindow.IngelogdeDokter = dokter;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ToonNavigatie();
        }
    }
}
