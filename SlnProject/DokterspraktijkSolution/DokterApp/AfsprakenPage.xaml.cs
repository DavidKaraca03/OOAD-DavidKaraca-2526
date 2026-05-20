using Lib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DokterApp
{
    /// <summary>
    /// Toont de afspraken van de ingelogde dokter voor een geselecteerde datum.
    /// </summary>
    public partial class AfsprakenPage : Page
    {
        public AfsprakenPage()
        {
            InitializeComponent();
        }

        // Stel vandaag in als standaard datum bij het laden van de pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Kalender.SelectedDate = DateTime.Today;
            LaadAfspraken(DateTime.Today);
        }

        // Laad afspraken voor de nieuw geselecteerde datum
        private void Kalender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Kalender.SelectedDate != null)
            {
                LaadAfspraken(Kalender.SelectedDate.Value);
            }
        }

        /// <summary>
        /// Haalt afspraken op voor de gegeven datum en toont ze als cards in het paneel.
        /// </summary>
        private void LaadAfspraken(DateTime datum)
        {
            // header en foutmelding resetten
            TxtFout.Text = string.Empty;
            TxtDatumHeader.Text = "Afspraken voor " + datum.ToString("dddd d MMMM yyyy");

            // paneel leegmaken voor nieuwe inhoud
            PnlAfspraken.Children.Clear();

            try
            {
                // afspraken ophalen voor de ingelogde dokter op de gekozen datum
                Dokter ingelogdeDokter = MainWindow.IngelogdeDokter;
                List<Afspraak> afspraken = Afspraak.GetByDokter(ingelogdeDokter.Id, datum);

                // melding tonen als er geen afspraken zijn
                if (afspraken.Count == 0)
                {
                    TextBlock lblLeeg = new TextBlock();
                    lblLeeg.Text = "Geen afspraken voor deze datum.";
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

            // tijdstip van de afspraak
            TextBlock lblTijdstip = new TextBlock();
            lblTijdstip.Text = afspraak.Moment.ToString("HH:mm");
            lblTijdstip.FontSize = 16;
            lblTijdstip.FontWeight = FontWeights.Bold;
            sp.Children.Add(lblTijdstip);

            // naam van de patiënt ophalen via computed property
            Patient patient = afspraak.Patient;
            TextBlock lblPatient = new TextBlock();
            lblPatient.Text = patient != null ? patient.ToString() : "(onbekende patiënt)";
            lblPatient.Margin = new Thickness(0, 4, 0, 0);
            sp.Children.Add(lblPatient);

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
    }
}
