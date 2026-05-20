using Lib;
using System.Windows;

namespace PatientApp
{
    /// <summary>
    /// Hoofdvenster van de PatientApp. Bevat navigatiepaneel en hoofdframe.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Bijhouden welke patiënt momenteel is ingelogd
        public static Patient IngelogdePatient { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        // Navigeer naar de loginpagina bij het openen van het venster
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HoofdFrame.Navigate(new LoginPage());
        }

        // Maakt het navigatiepaneel zichtbaar na succesvolle login
        public void ToonNavigatie()
        {
            NavPanel.Visibility = Visibility.Visible;
        }
    }
}
