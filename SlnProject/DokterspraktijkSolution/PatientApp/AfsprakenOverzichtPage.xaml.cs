using Lib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PatientApp
{
    /// <summary>
    /// Toont een overzicht van alle afspraken van de ingelogde patiënt als kaarten.
    /// </summary>
    public partial class AfsprakenOverzichtPage : Page
    {
        public AfsprakenOverzichtPage()
        {
            InitializeComponent();
        }

        // Laad de afspraken bij het openen van de pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadAfspraken();
        }

        /// <summary>
        /// Haalt de afspraken op van de ingelogde patiënt en toont ze als cards.
        /// </summary>
        private void LaadAfspraken()
        {
            // foutmelding en paneel resetten
            TxtFout.Text = string.Empty;
            PnlAfspraken.Children.Clear();

            try
            {
                // afspraken ophalen voor de ingelogde patiënt
                Patient patient = MainWindow.IngelogdePatient;
                List<Afspraak> afspraken = Afspraak.GetByPatient(patient.Id);

                // melding tonen als er geen afspraken zijn
                if (afspraken.Count == 0)
                {
                    TextBlock lblLeeg = new TextBlock();
                    lblLeeg.Text = "U heeft nog geen afspraken.";
                    lblLeeg.Foreground = Brushes.Gray;
                    lblLeeg.FontStyle = FontStyles.Italic;
                    PnlAfspraken.Children.Add(lblLeeg);
                    return;
                }

                // voor elke afspraak een card aanmaken en toevoegen
                foreach (Afspraak afspraak in afspraken)
                {
                    Border kaart = MaakAfspraakKaart(afspraak);
                    PnlAfspraken.Children.Add(kaart);
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van afspraken: " + ex.Message;
            }
        }

        /// <summary>
        /// Maakt een visuele card aan voor één afspraak.
        /// </summary>
        private Border MaakAfspraakKaart(Afspraak afspraak)
        {
            // buitenrand van de card
            Border kaart = new Border();
            kaart.BorderBrush = new SolidColorBrush(Color.FromRgb(210, 210, 210));
            kaart.BorderThickness = new Thickness(1);
            kaart.CornerRadius = new CornerRadius(4);
            kaart.Margin = new Thickness(0, 0, 0, 10);
            kaart.Padding = new Thickness(14);
            kaart.Background = Brushes.White;

            // inhoud van de card
            StackPanel sp = new StackPanel();

            // datum en tijdstip van de afspraak
            TextBlock lblDatum = new TextBlock();
            lblDatum.Text = afspraak.Moment.ToString("dddd d MMMM yyyy 'om' HH:mm");
            lblDatum.FontSize = 15;
            lblDatum.FontWeight = FontWeights.Bold;
            sp.Children.Add(lblDatum);

            // naam van de dokter ophalen via computed property
            Dokter dokter = afspraak.Dokter;
            TextBlock lblDokter = new TextBlock();
            lblDokter.Text = dokter != null ? "Dr. " + dokter.ToString() : "(onbekende dokter)";
            lblDokter.Margin = new Thickness(0, 4, 0, 0);
            sp.Children.Add(lblDokter);

            // klacht (reden van het bezoek)
            TextBlock lblKlacht = new TextBlock();
            lblKlacht.Text = afspraak.Klacht;
            lblKlacht.Foreground = Brushes.Gray;
            lblKlacht.TextWrapping = TextWrapping.Wrap;
            lblKlacht.Margin = new Thickness(0, 2, 0, 0);
            sp.Children.Add(lblKlacht);

            kaart.Child = sp;
            return kaart;
        }

        // Navigeer naar het formulier voor een nieuwe afspraak
        private void BtnNieuweAfspraak_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NieuweAfspraakPage());
        }
    }
}
