using Lib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DokterApp
{
    /// <summary>
    /// Toont een gridweergave van alle patiënten met kolomheaders.
    /// </summary>
    public partial class PatientenOverzichtPage : Page
    {
        public PatientenOverzichtPage()
        {
            InitializeComponent();
        }

        // Laad alle patiënten bij het openen van de pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LaadPatienten(Patient.GetAll());
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van patiënten: " + ex.Message;
            }
        }

        // Zoek patiënten op naam
        private void BtnZoeken_Click(object sender, RoutedEventArgs e)
        {
            TxtFout.Text = string.Empty;
            string zoekterm = TxtZoek.Text.Trim();

            try
            {
                if (string.IsNullOrWhiteSpace(zoekterm))
                {
                    LaadPatienten(Patient.GetAll());
                }
                else
                {
                    LaadPatienten(Patient.GetAll(zoekterm));
                }
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij zoeken: " + ex.Message;
            }
        }

        // Zoekterm wissen en alle patiënten tonen
        private void BtnAlles_Click(object sender, RoutedEventArgs e)
        {
            TxtFout.Text = string.Empty;
            TxtZoek.Text = string.Empty;

            try
            {
                LaadPatienten(Patient.GetAll());
            }
            catch (Exception ex)
            {
                TxtFout.Text = "Fout bij het laden van patiënten: " + ex.Message;
            }
        }

        /// <summary>
        /// Vult het patiëntenpaneel met een rij per patiënt in de lijst.
        /// </summary>
        private void LaadPatienten(List<Patient> patienten)
        {
            PnlPatienten.Children.Clear();

            if (patienten.Count == 0)
            {
                TextBlock lblLeeg = new TextBlock();
                lblLeeg.Text = "Geen patiënten gevonden.";
                lblLeeg.Foreground = Brushes.Gray;
                lblLeeg.FontStyle = FontStyles.Italic;
                lblLeeg.Margin = new Thickness(10, 8, 0, 0);
                PnlPatienten.Children.Add(lblLeeg);
                return;
            }

            int rijIndex = 0;
            foreach (Patient p in patienten)
            {
                Border rij = MaakPatientRij(p, rijIndex);
                PnlPatienten.Children.Add(rij);
                rijIndex++;
            }
        }

        /// <summary>
        /// Maakt één tabelrij aan voor een patiënt met kolommen: naam, geboortedatum, gsm, e-mail, detailknop.
        /// </summary>
        private Border MaakPatientRij(Patient patient, int rijIndex)
        {
            // afwisselende achtergrond voor leesbaarheid
            Color achtergrond = (rijIndex % 2 == 0)
                ? Colors.White
                : Color.FromRgb(248, 249, 250);

            Border rand = new Border();
            rand.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            rand.BorderThickness = new Thickness(0, 0, 0, 1);
            rand.Padding = new Thickness(10, 8, 10, 8);
            rand.Background = new SolidColorBrush(achtergrond);

            Grid rij = new Grid();
            rij.HorizontalAlignment = HorizontalAlignment.Stretch;
            rij.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            rij.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(115) });
            rij.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(135) });
            rij.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            rij.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });

            // naam
            TextBlock lblNaam = new TextBlock();
            lblNaam.Text = patient.Voornaam + " " + patient.Achternaam;
            lblNaam.VerticalAlignment = VerticalAlignment.Center;
            lblNaam.TextTrimming = TextTrimming.CharacterEllipsis;
            Grid.SetColumn(lblNaam, 0);
            rij.Children.Add(lblNaam);

            // geboortedatum
            TextBlock lblGeboortedatum = new TextBlock();
            lblGeboortedatum.Text = patient.Geboortedatum.ToString("dd/MM/yyyy");
            lblGeboortedatum.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(lblGeboortedatum, 1);
            rij.Children.Add(lblGeboortedatum);

            // gsm
            TextBlock lblGsm = new TextBlock();
            lblGsm.Text = string.IsNullOrEmpty(patient.Gsm) ? "—" : patient.Gsm;
            lblGsm.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(lblGsm, 2);
            rij.Children.Add(lblGsm);

            // e-mail
            TextBlock lblEmail = new TextBlock();
            lblEmail.Text = patient.Email;
            lblEmail.VerticalAlignment = VerticalAlignment.Center;
            lblEmail.TextTrimming = TextTrimming.CharacterEllipsis;
            Grid.SetColumn(lblEmail, 3);
            rij.Children.Add(lblEmail);

            // detailknop
            Button btnDetails = new Button();
            btnDetails.Content = "Details";
            btnDetails.Padding = new Thickness(8, 3, 8, 3);
            btnDetails.Tag = patient.Id;
            btnDetails.Click += BtnDetails_Click;
            Grid.SetColumn(btnDetails, 4);
            rij.Children.Add(btnDetails);

            rand.Child = rij;
            return rand;
        }

        // Navigeer naar de detailpagina van de geselecteerde patiënt
        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int patientId = Convert.ToInt32(btn.Tag);
            NavigationService.Navigate(new PatientDetailsPage(patientId));
        }

        // Navigeer naar het formulier voor een nieuwe patiënt
        private void BtnNieuw_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientNieuwPage());
        }
    }
}
