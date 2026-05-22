using System.Windows;
using System.Windows.Controls;

namespace PatientApp
{
    /// <summary>
    /// Startpagina van de PatientApp. Toont een beschrijving van de applicatie
    /// en een knop om naar de loginpagina te navigeren.
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        // Navigeer naar de loginpagina
        private void BtnAanmelden_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}
