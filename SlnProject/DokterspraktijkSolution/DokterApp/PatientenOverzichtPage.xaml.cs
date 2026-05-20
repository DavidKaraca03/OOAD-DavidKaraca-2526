using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DokterApp
{
    /// <summary>
    /// Toont een overzicht van alle patiënten als dynamische kaarten in een WrapPanel.
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

        // Zoek patiënten op naam en laad de resultaten
        private void BtnZoeken_Click(object sender, RoutedEventArgs e)
        {
            TxtFout.Text = string.Empty;
            string zoekterm = TxtZoek.Text.Trim();

            try
            {
                // lege zoekterm toont alle patiënten
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
        /// Vult het WrapPanel met een kaart voor elke patiënt in de lijst.
        /// </summary>
        private void LaadPatienten(List<Patient> patienten)
        {
            // paneel leegmaken
            PnlPatienten.Children.Clear();

            // melding tonen als er geen resultaten zijn
            if (patienten.Count == 0)
            {
                TextBlock lblLeeg = new TextBlock();
                lblLeeg.Text = "Geen patiënten gevonden.";
                lblLeeg.Foreground = Brushes.Gray;
                lblLeeg.FontStyle = FontStyles.Italic;
                lblLeeg.Margin = new Thickness(5);
                PnlPatienten.Children.Add(lblLeeg);
                return;
            }

            // voor elke patiënt een kaart aanmaken
            foreach (Patient p in patienten)
            {
                Border kaart = MaakPatientKaart(p);
                PnlPatienten.Children.Add(kaart);
            }
        }

        /// <summary>
        /// Maakt een visuele card aan voor één patiënt met foto, naam en detailknop.
        /// </summary>
        private Border MaakPatientKaart(Patient patient)
        {
            // buitenrand van de card
            Border kaart = new Border();
            kaart.Width = 180;
            kaart.Margin = new Thickness(6);
            kaart.BorderBrush = new SolidColorBrush(Color.FromRgb(210, 210, 210));
            kaart.BorderThickness = new Thickness(1);
            kaart.CornerRadius = new CornerRadius(6);
            kaart.Padding = new Thickness(12);
            kaart.Background = Brushes.White;

            // inhoud van de card
            StackPanel sp = new StackPanel();
            sp.HorizontalAlignment = HorizontalAlignment.Center;

            // profielfoto
            Image img = new Image();
            img.Width = 90;
            img.Height = 90;
            img.Margin = new Thickness(0, 0, 0, 8);
            img.HorizontalAlignment = HorizontalAlignment.Center;

            // byte[] omzetten naar BitmapImage indien foto aanwezig
            if (patient.Profielfotodata != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(patient.Profielfotodata))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                }
                img.Source = bitmap;
            }
            sp.Children.Add(img);

            // volledige naam van de patiënt
            TextBlock lblNaam = new TextBlock();
            lblNaam.Text = patient.ToString();
            lblNaam.TextWrapping = TextWrapping.Wrap;
            lblNaam.HorizontalAlignment = HorizontalAlignment.Center;
            lblNaam.FontWeight = FontWeights.SemiBold;
            sp.Children.Add(lblNaam);

            // knop om naar de detailpagina te navigeren
            Button btnDetails = new Button();
            btnDetails.Content = "Details";
            btnDetails.Margin = new Thickness(0, 10, 0, 0);
            btnDetails.Padding = new Thickness(0, 5, 0, 5);
            btnDetails.Tag = patient.Id;
            btnDetails.Click += BtnDetails_Click;
            sp.Children.Add(btnDetails);

            kaart.Child = sp;
            return kaart;
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
