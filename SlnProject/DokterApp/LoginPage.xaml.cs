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
            // foutpaneel verbergen bij nieuwe poging
            PnlFout.Visibility = Visibility.Collapsed;
            TxtFout.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(TxtEmail.Text) || TxtPaswoord.Password.Length == 0)
            {
                TxtFout.Text = "Vul uw e-mail en wachtwoord in.";
                PnlFout.Visibility = Visibility.Visible;
                return;
            }

            Dokter dokter = Dokter.Login(TxtEmail.Text.Trim(), TxtPaswoord.Password);

            if (dokter == null)
            {
                TxtFout.Text = "Ongeldig e-mailadres of wachtwoord.";
                PnlFout.Visibility = Visibility.Visible;
                return;
            }

            MainWindow.IngelogdeDokter = dokter;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ToonNavigatie();
        }
    }
}
