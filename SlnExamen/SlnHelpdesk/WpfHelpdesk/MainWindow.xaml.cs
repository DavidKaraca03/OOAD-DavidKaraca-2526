using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CLHelpdesk;

namespace WpfHelpdesk
{
    /// <summary>
    /// Code-behind voor het hoofdvenster van de Helpdesk-applicatie.
    /// </summary>
    public partial class MainWindow : Window
    {
        // alle tickets uit het CSV-bestand (ongefilterd)
        private List<Ticket> _alleTickets = new List<Ticket>();

        // het huidig geselecteerde ticket in de listbox
        private Ticket _huidigTicket = null;

        /// <summary>
        /// Constructor: initialiseer venster, stel CSV-pad in en laad tickets.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // CSV-pad instellen via de class library
            string csvPad = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "helpdesk_tickets.csv");
            TicketRepository.StelPadIn(csvPad);

            // alle tickets laden bij opstarten
            LaadTickets();
        }

        /// <summary>
        /// Haalt alle tickets op uit het CSV-bestand en past de filters toe.
        /// </summary>
        private void LaadTickets()
        {
            // tickets ophalen via de class library
            _alleTickets = TicketRepository.GetAll();

            // gefilterde lijst tonen in de listbox
            PasFiltersToe();
        }

        /// <summary>
        /// Past de actieve filters toe op de volledige ticketlijst en vult de listbox.
        /// </summary>
        private void PasFiltersToe()
        {
            // gefilterde lijst aanmaken
            List<Ticket> gefilterd = new List<Ticket>();

            // huidige filterwaarden ophalen
            ComboBoxItem geselecteerdPrioriteit = cmbPrioriteit.SelectedItem as ComboBoxItem;
            string filterPrioriteit = geselecteerdPrioriteit != null ? geselecteerdPrioriteit.Content.ToString() : "Alle";
            string filterMelder = txtFilterMelder.Text.Trim().ToLower();
            bool alleenOpen = chkAlleenOpen.IsChecked == true;

            // elk ticket controleren op de actieve filters
            foreach (Ticket t in _alleTickets)
            {
                // filter: alleen open tickets
                if (alleenOpen && t.IsAfgesloten)
                {
                    continue;
                }

                // filter: prioriteit
                if (filterPrioriteit != "Alle" && t.Prioriteit.ToString() != filterPrioriteit)
                {
                    continue;
                }

                // filter: melder (gedeeltelijke overeenkomst)
                if (!string.IsNullOrEmpty(filterMelder) && !t.Melder.ToLower().Contains(filterMelder))
                {
                    continue;
                }

                // ticket voldoet aan alle filters
                gefilterd.Add(t);
            }

            // listbox verversen met de gefilterde resultaten
            lbxTickets.ItemsSource = null;
            lbxTickets.ItemsSource = gefilterd;
        }

        /// <summary>
        /// Toont de details van het geselecteerde ticket en activeert de afsluitknop indien nodig.
        /// </summary>
        private void LbxTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // geselecteerd ticket ophalen
            _huidigTicket = lbxTickets.SelectedItem as Ticket;

            // geen selectie: standaardtekst tonen en knop uitschakelen
            if (_huidigTicket == null)
            {
                txtDetails.Text = "Selecteer een ticket om de details te zien.";
                btnAfsluiten.IsEnabled = false;
                return;
            }

            // detailinformatie tonen via de GeefInfo-methode
            txtDetails.Text = _huidigTicket.GeefInfo();

            // afsluitknop alleen actief als het ticket nog open is
            btnAfsluiten.IsEnabled = !_huidigTicket.IsAfgesloten;
        }

        /// <summary>
        /// Verwerkt een wijziging in een filter en herlaadt de gefilterde lijst.
        /// </summary>
        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            PasFiltersToe();
        }

        /// <summary>
        /// Past het label aan (Toestel of Applicatie) als het type wijzigt.
        /// </summary>
        private void CmbNieuwType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // veiligheidscheck: label kan null zijn tijdens initialisatie
            if (lblExtraInfo == null)
            {
                return;
            }

            // label aanpassen op basis van het gekozen type
            ComboBoxItem geselecteerdType = cmbNieuwType.SelectedItem as ComboBoxItem;
            string type = geselecteerdType != null ? geselecteerdType.Content.ToString() : "Hardware";
            lblExtraInfo.Text = type == "Hardware" ? "Toestel *:" : "Applicatie *:";
        }

        /// <summary>
        /// Valideert het formulier en voegt een nieuw ticket toe aan het CSV-bestand.
        /// </summary>
        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            // foutmelding leegmaken
            lblFout.Text = "";

            // veldwaarden ophalen en opschonen
            string titel = txtNieuwTitel.Text.Trim();
            string melder = txtNieuwMelder.Text.Trim();
            string extraInfo = txtNieuwExtraInfo.Text.Trim();

            ComboBoxItem geselecteerdType = cmbNieuwType.SelectedItem as ComboBoxItem;
            string type = geselecteerdType != null ? geselecteerdType.Content.ToString() : "Hardware";

            ComboBoxItem geselecteerdPrioriteit = cmbNieuwPrioriteit.SelectedItem as ComboBoxItem;
            string prioriteitStr = geselecteerdPrioriteit != null ? geselecteerdPrioriteit.Content.ToString() : "Normaal";

            // validatie: titel verplicht
            if (string.IsNullOrEmpty(titel))
            {
                lblFout.Text = "Titel is verplicht.";
                return;
            }

            // validatie: melder verplicht
            if (string.IsNullOrEmpty(melder))
            {
                lblFout.Text = "Melder is verplicht.";
                return;
            }

            // validatie: toestel of applicatie verplicht
            if (string.IsNullOrEmpty(extraInfo))
            {
                string veld = type == "Hardware" ? "Toestel" : "Applicatie";
                lblFout.Text = $"{veld} is verplicht.";
                return;
            }

            // prioriteit omzetten naar enum-waarde
            Enum.TryParse(prioriteitStr, out TicketPrioriteit prioriteit);

            // volgend beschikbaar ID bepalen
            int nieuwId = TicketRepository.VolgendeId();

            // juist ticketobject aanmaken op basis van het gekozen type
            Ticket nieuwTicket;
            if (type == "Hardware")
            {
                nieuwTicket = new HardwareTicket(nieuwId, titel, melder, prioriteit, DateTime.Now, extraInfo);
            }
            else
            {
                nieuwTicket = new SoftwareTicket(nieuwId, titel, melder, prioriteit, DateTime.Now, extraInfo);
            }

            // ticket opslaan via de class library
            TicketRepository.VoegToe(nieuwTicket);

            // formulier leegmaken na opslaan
            txtNieuwTitel.Text = "";
            txtNieuwMelder.Text = "";
            txtNieuwExtraInfo.Text = "";
            cmbNieuwPrioriteit.SelectedIndex = 1;
            cmbNieuwType.SelectedIndex = 0;

            // overzicht herladen
            LaadTickets();

            // bevestigingsbericht tonen
            MessageBox.Show($"Ticket #{nieuwId} succesvol toegevoegd!", "Ticket toegevoegd",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Sluit het geselecteerde ticket af na bevestiging van de gebruiker.
        /// </summary>
        private void BtnAfsluiten_Click(object sender, RoutedEventArgs e)
        {
            // veiligheidscheck: geen ticket geselecteerd of al afgesloten
            if (_huidigTicket == null || _huidigTicket.IsAfgesloten)
            {
                return;
            }

            // bevestiging vragen aan de gebruiker
            MessageBoxResult bevestiging = MessageBox.Show(
                $"Ben je zeker dat je ticket #{_huidigTicket.Id} wilt afsluiten?\n\"{_huidigTicket.Titel}\"",
                "Ticket afsluiten",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            // ticket afsluiten als de gebruiker bevestigt
            if (bevestiging == MessageBoxResult.Yes)
            {
                // afsluiten via de class library
                TicketRepository.SluitAf(_huidigTicket.Id);

                // overzicht herladen
                LaadTickets();

                // detailpaneel bijwerken
                txtDetails.Text = "Ticket afgesloten.";
                btnAfsluiten.IsEnabled = false;

                // bevestigingsbericht tonen
                MessageBox.Show("Ticket succesvol afgesloten!", "Afgesloten",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
