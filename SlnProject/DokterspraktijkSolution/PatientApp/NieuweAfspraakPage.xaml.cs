using Lib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PatientApp
{
    /// <summary>
    /// Formulier waarmee de patiënt een nieuwe afspraak kan boeken bij een dokter.
    /// </summary>
    public partial class NieuweAfspraakPage : Page
    {
        // Bijhouden van de geladen dokterlijst zodat we het geselecteerde id kunnen ophalen
        private List<Dokter> _dokters;

        public NieuweAfspraakPage()
        {
            InitializeComponent();
        }

        // Laad de dokterlijst bij het openen van de pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadDokters();
        }

        /// <summary>
        /// Haalt alle dokters op en vult de ListBox.
        /// </summary>
        private void LaadDokters()
        {
            TxtFout.Text = string.Empty;

            try
            {
                // alle dokters ophalen
                _dokters = Dokter.GetAll();

                // ListBox vullen met de naam van elke dokter
                LstDokters.Items.Clear();
                foreach (Dokter dokter in _dokters)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = "Dr. " + dokter.ToString();
                    item.Tag = dokter.Id;
                    LstDokters.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van dokters: " + ex.Message;
            }
        }

        /// <summary>
        /// Valideert alle velden en geeft true terug als alles correct is ingevuld.
        /// </summary>
        private bool ValideerFormulier()
        {
            TxtFout.Text = string.Empty;

            // dokter moet geselecteerd zijn
            if (LstDokters.SelectedItem == null)
            {
                TxtFout.Text = "Kies een dokter.";
                return false;
            }

            // datum is verplicht
            if (string.IsNullOrWhiteSpace(TxtDatum.Text))
            {
                TxtFout.Text = "Datum is verplicht.";
                return false;
            }

            // tijdstip is verplicht
            if (string.IsNullOrWhiteSpace(TxtTijdstip.Text))
            {
                TxtFout.Text = "Tijdstip is verplicht.";
                return false;
            }

            // klacht is verplicht
            if (string.IsNullOrWhiteSpace(TxtKlacht.Text))
            {
                TxtFout.Text = "Geef een klacht of reden van bezoek op.";
                return false;
            }

            return true;
        }

        // Sla de afspraak op na validatie
        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            // formulier valideren voor we verder gaan
            if (!ValideerFormulier())
            {
                return;
            }

            try
            {
                // datum en tijdstip samenvoegen tot één DateTime
                string datumTijdTekst = TxtDatum.Text.Trim() + " " + TxtTijdstip.Text.Trim();
                DateTime moment = DateTime.ParseExact(datumTijdTekst, "dd/MM/yyyy HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture);

                // geselecteerde dokter-id ophalen via Tag
                ListBoxItem geselecteerdItem = (ListBoxItem)LstDokters.SelectedItem;
                int dokterId = Convert.ToInt32(geselecteerdItem.Tag);

                // nieuwe afspraak aanmaken en opslaan
                Afspraak nieuweAfspraak = new Afspraak();
                nieuweAfspraak.Moment    = moment;
                nieuweAfspraak.Klacht    = TxtKlacht.Text.Trim();
                nieuweAfspraak.PatientId = MainWindow.IngelogdePatient.Id;
                nieuweAfspraak.DokterId  = dokterId;
                nieuweAfspraak.InsertInDb();

                // terugkeren naar het afsprakenoverzicht
                NavigationService.Navigate(new AfsprakenOverzichtPage());
            }
            catch (FormatException)
            {
                TxtFout.Text = "Ongeldige datum of tijdstip. Gebruik het formaat dd/MM/yyyy en HH:mm.";
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het opslaan: " + ex.Message;
            }
        }

        // Annuleren: terug naar het afsprakenoverzicht zonder op te slaan
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AfsprakenOverzichtPage());
        }
    }
}
