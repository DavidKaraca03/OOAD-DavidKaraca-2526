using Lib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PatientApp
{
    /// <summary>
    /// Formulier waarmee de patiënt een nieuwe afspraak kan boeken bij een dokter.
    /// Het tijdstip wordt gekozen uit een ComboBox met nog beschikbare slots (08:00–17:30, interval 30 min).
    /// </summary>
    public partial class NieuweAfspraakPage : Page
    {
        // Interval in minuten tussen opeenvolgende tijdsloten
        private const int TIJDSLOT_INTERVAL = 30;

        // Bijhouden van de geladen dokterlijst
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
                _dokters = Dokter.GetAll();

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

        // Toon de basisinfo van de geselecteerde dokter en herlaad de tijdsloten
        private void LstDokters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstDokters.SelectedItem == null)
            {
                PnlDokterInfo.Visibility = Visibility.Collapsed;
                CboTijdstip.Items.Clear();
                return;
            }

            ListBoxItem geselecteerdItem = (ListBoxItem)LstDokters.SelectedItem;
            int dokterId = Convert.ToInt32(geselecteerdItem.Tag);

            Dokter gevonden = null;
            foreach (Dokter dokter in _dokters)
            {
                if (dokter.Id == dokterId)
                {
                    gevonden = dokter;
                }
            }

            if (gevonden == null)
            {
                PnlDokterInfo.Visibility = Visibility.Collapsed;
                return;
            }

            LblDokterNaam.Text             = "Dr. " + gevonden.Voornaam + " " + gevonden.Achternaam;
            LblDokterRiziv.Text            = Convert.ToString(gevonden.Rizivnummer);
            LblDokterGeconventioneerd.Text = gevonden.IsGeconventioneerd ? "Ja" : "Nee";
            PnlDokterInfo.Visibility = Visibility.Visible;

            // tijdsloten herladen voor de nieuw geselecteerde dokter
            LaadTijdsloten();
        }

        // Herlaad de tijdsloten wanneer de datum verandert
        private void DpDatum_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LaadTijdsloten();
        }

        /// <summary>
        /// Genereert de beschikbare tijdsloten (08:00–17:30, stap 30 min) voor de geselecteerde
        /// dokter en datum. Reeds geboekte en verleden sloten worden overgeslagen.
        /// </summary>
        private void LaadTijdsloten()
        {
            CboTijdstip.Items.Clear();

            // beide selecties zijn nodig om sloten te kunnen bepalen
            if (LstDokters.SelectedItem == null || DpDatum.SelectedDate == null)
            {
                return;
            }

            ListBoxItem geselecteerdItem = (ListBoxItem)LstDokters.SelectedItem;
            int dokterId = Convert.ToInt32(geselecteerdItem.Tag);
            DateTime datum = DpDatum.SelectedDate.Value.Date;

            try
            {
                // bestaande afspraken ophalen voor deze dokter op deze datum
                List<Afspraak> bestaandeAfspraken = Afspraak.GetByDokter(dokterId, datum);

                DateTime startTijd    = new DateTime(datum.Year, datum.Month, datum.Day, 8, 0, 0);
                DateTime eindTijd     = new DateTime(datum.Year, datum.Month, datum.Day, 17, 30, 0);
                DateTime huidigMoment = DateTime.Now;
                DateTime tijdslot     = startTijd;

                while (tijdslot <= eindTijd)
                {
                    // verleden tijdsloten niet aanbieden
                    if (tijdslot > huidigMoment)
                    {
                        // controleren of dit slot al geboekt is
                        bool geboekt = false;
                        foreach (Afspraak a in bestaandeAfspraken)
                        {
                            if (a.Moment.Hour == tijdslot.Hour && a.Moment.Minute == tijdslot.Minute)
                            {
                                geboekt = true;
                            }
                        }

                        if (!geboekt)
                        {
                            CboTijdstip.Items.Add(tijdslot.ToString("HH:mm"));
                        }
                    }

                    tijdslot = tijdslot.AddMinutes(TIJDSLOT_INTERVAL);
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van tijdsloten: " + ex.Message;
            }
        }

        /// <summary>
        /// Valideert alle velden en geeft true terug als alles correct is ingevuld.
        /// </summary>
        private bool ValideerFormulier()
        {
            TxtFout.Text = string.Empty;

            if (LstDokters.SelectedItem == null)
            {
                TxtFout.Text = "Kies een dokter.";
                return false;
            }

            if (DpDatum.SelectedDate == null)
            {
                TxtFout.Text = "Datum is verplicht.";
                return false;
            }

            if (DpDatum.SelectedDate.Value.Date < DateTime.Today)
            {
                TxtFout.Text = "De geselecteerde datum ligt in het verleden. Kies een datum vanaf vandaag.";
                return false;
            }

            if (CboTijdstip.Items.Count == 0)
            {
                TxtFout.Text = "Er zijn geen beschikbare tijdsloten voor deze dokter en datum.";
                return false;
            }

            if (CboTijdstip.SelectedItem == null)
            {
                TxtFout.Text = "Kies een tijdstip uit de lijst.";
                return false;
            }

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
            if (!ValideerFormulier())
            {
                return;
            }

            try
            {
                DateTime gekozenDatum = DpDatum.SelectedDate.Value;
                string tijdstipTekst = CboTijdstip.SelectedItem.ToString();

                DateTime gekozenTijdstip = DateTime.ParseExact(
                    tijdstipTekst, "HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture);

                DateTime moment = new DateTime(
                    gekozenDatum.Year, gekozenDatum.Month, gekozenDatum.Day,
                    gekozenTijdstip.Hour, gekozenTijdstip.Minute, 0);

                ListBoxItem geselecteerdItem = (ListBoxItem)LstDokters.SelectedItem;
                int dokterId = Convert.ToInt32(geselecteerdItem.Tag);

                Afspraak nieuweAfspraak = new Afspraak();
                nieuweAfspraak.Moment    = moment;
                nieuweAfspraak.Klacht    = TxtKlacht.Text.Trim();
                nieuweAfspraak.PatientId = MainWindow.IngelogdePatient.Id;
                nieuweAfspraak.DokterId  = dokterId;
                nieuweAfspraak.InsertInDb();

                NavigationService.Navigate(new AfsprakenOverzichtPage());
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het opslaan: " + ex.Message;
            }
        }

        // Annuleren: terug naar het afsprakenoverzicht
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AfsprakenOverzichtPage());
        }
    }
}
