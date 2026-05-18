using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfVcardEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentFilePath = null;
        private bool isModified = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow wnd = new AboutWindow();
            wnd.Owner = this;
            wnd.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Ben je zeker dat je de applicatie wil afsluiten?",
                "Toepassing sluiten",
                MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "vCard files (*.vcf)|*.vcf|All files (*.*)|*.*";

            if (dlg.ShowDialog() != true) return;

            string[] lines;
            try
            {
                lines = File.ReadAllLines(dlg.FileName);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Kan bestand niet lezen: {ex.Message}", "FOUT", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Geen toegang tot bestand: {ex.Message}", "FOUT", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Onverwachte fout: {ex.Message}", "FOUT", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            currentFilePath = dlg.FileName;
            mimSave.IsEnabled = true;

            string photoBase64 = "";
            bool readingPhoto = false;

            foreach (string line in lines)
            {
                if (readingPhoto)
                {
                    if (line.StartsWith(" ") || line.StartsWith("\t"))
                    {
                        photoBase64 += line.Trim();
                        continue;
                    }
                    else
                    {
                        LoadPhotoFromBase64(photoBase64);
                        readingPhoto = false;
                    }
                }

                if (line.StartsWith("PHOTO"))
                {
                    int colonIndex = line.LastIndexOf(':');
                    if (colonIndex > -1)
                    {
                        photoBase64 = line.Substring(colonIndex + 1).Trim();
                        readingPhoto = true;
                    }
                }
                else if (line.StartsWith("N;") || line.StartsWith("N:"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        string data = line.Substring(colonIndex + 1);
                        string[] parts = data.Split(';');
                        if (parts.Length >= 2)
                        {
                            txtLastname.Text = parts[0];
                            txtFirstname.Text = parts[1];
                        }
                    }
                }
                else if (line.StartsWith("BDAY:"))
                {
                    string dateStr = line.Substring(5).Trim();
                    if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime birthday))
                    {
                        datBirthday.SelectedDate = birthday;
                    }
                }
                else if (line.StartsWith("GENDER:"))
                {
                    string gender = line.Substring(7).Trim().ToUpper();
                    if (gender == "M")
                        rdbMan.IsChecked = true;
                    else if (gender == "F")
                        rdbVrouw.IsChecked = true;
                    else
                        rdbOnbekend.IsChecked = true;
                }
                else if (line.StartsWith("EMAIL") && line.Contains("type=HOME"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtEmail.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
                else if (line.StartsWith("EMAIL") && line.Contains("type=WORK"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtWerkEmail.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
                else if (line.StartsWith("TEL") && line.Contains("TYPE=HOME"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtPhone.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
                else if (line.StartsWith("TEL") && line.Contains("TYPE=WORK"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtWerkTelefoon.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
                else if (line.StartsWith("ORG"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtBedrijf.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
                else if (line.StartsWith("TITLE"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtJobTitel.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
                else if (line.StartsWith("X-SOCIALPROFILE") && line.Contains("TYPE=facebook"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtFacebook.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
                else if (line.StartsWith("X-SOCIALPROFILE") && line.Contains("TYPE=linkedin"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtLinkedIn.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
                else if (line.StartsWith("X-SOCIALPROFILE") && line.Contains("TYPE=instagram"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtInstagram.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
                else if (line.StartsWith("X-SOCIALPROFILE") && line.Contains("TYPE=youtube"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > -1)
                    {
                        txtYouTube.Text = line.Substring(colonIndex + 1).Trim();
                    }
                }
            }

            if (readingPhoto && !string.IsNullOrEmpty(photoBase64))
            {
                LoadPhotoFromBase64(photoBase64);
            }

            txtKaart.Text = $"huidige kaart: {currentFilePath}";
            UpdatePercentage();
            isModified = false;
        }


        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "vCard files (*.vcf)|*.vcf|All files (*.*)|*.*";

            if (dlg.ShowDialog() != true) return;

            currentFilePath = dlg.FileName;
            mimSave.IsEnabled = true;
            SaveVCardToFile(currentFilePath);

            txtKaart.Text = $"huidige kaart: {currentFilePath}";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveVCardToFile(currentFilePath);
            MessageBox.Show("vCard succesvol opgeslagen.", "Opgeslagen", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveVCardToFile(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCARD");
            sb.AppendLine("VERSION:3.0");

            if (!string.IsNullOrWhiteSpace(txtFirstname.Text) || !string.IsNullOrWhiteSpace(txtLastname.Text))
            {
                sb.AppendLine($"FN:{txtFirstname.Text} {txtLastname.Text}".Trim());
            }

            // Persoonlijk
            if (!string.IsNullOrWhiteSpace(txtLastname.Text) || !string.IsNullOrWhiteSpace(txtFirstname.Text))
            {
                sb.AppendLine($"N:{txtLastname.Text};{txtFirstname.Text};;;");
            }
            if (datBirthday.SelectedDate.HasValue)
            {
                sb.AppendLine("BDAY:" + datBirthday.SelectedDate.Value.ToString("yyyyMMdd"));
            }
            if (rdbMan.IsChecked == true)
            {
                sb.AppendLine("GENDER:M");
            }
            else if (rdbVrouw.IsChecked == true)
            {
                sb.AppendLine("GENDER:F");
            }

            // AI: foto opslaan als base64 in VCF met correcte regelafbrekingen
            if (imgFoto.Source != null)
            {
                try
                {
                    BitmapSource bitmapSource = (BitmapSource)imgFoto.Source;
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        string base64 = Convert.ToBase64String(ms.ToArray());

                        // Eerste regel
                        sb.AppendLine("PHOTO;ENCODING=BASE64;TYPE=image/jpeg:");

                        // Base64 opsplitsen in stukken van 76 tekens, elke regel begint met spatie
                        int chunkSize = 76;
                        for (int i = 0; i < base64.Length; i += chunkSize)
                        {
                            string chunk = base64.Substring(i, Math.Min(chunkSize, base64.Length - i));
                            sb.AppendLine(" " + chunk);
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show($"Kan foto niet verwerken: {ex.Message}", "FOUT",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Kan foto niet lezen: {ex.Message}", "FOUT",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Onverwachte fout bij foto: {ex.Message}", "FOUT",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                sb.AppendLine($"EMAIL;CHARSET=UTF-8;type=HOME,INTERNET:{txtEmail.Text}");
            }
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                sb.AppendLine($"TEL;TYPE=HOME,VOICE:{txtPhone.Text}");
            }

            // Werk
            if (!string.IsNullOrWhiteSpace(txtBedrijf.Text))
            {
                sb.AppendLine($"ORG;CHARSET=UTF-8:{txtBedrijf.Text}");
            }
            if (!string.IsNullOrWhiteSpace(txtJobTitel.Text))
            {
                sb.AppendLine($"TITLE;CHARSET=UTF-8:{txtJobTitel.Text}");
            }
            if (!string.IsNullOrWhiteSpace(txtWerkEmail.Text))
            {
                sb.AppendLine($"EMAIL;CHARSET=UTF-8;type=WORK,INTERNET:{txtWerkEmail.Text}");
            }
            if (!string.IsNullOrWhiteSpace(txtWerkTelefoon.Text))
            {
                sb.AppendLine($"TEL;TYPE=WORK,VOICE:{txtWerkTelefoon.Text}");
            }

            // Sociaal
            if (!string.IsNullOrWhiteSpace(txtFacebook.Text))
            {
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=facebook:{txtFacebook.Text}");
            }
            if (!string.IsNullOrWhiteSpace(txtLinkedIn.Text))
            {
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=linkedin:{txtLinkedIn.Text}");
            }
            if (!string.IsNullOrWhiteSpace(txtInstagram.Text))
            {
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=instagram:{txtInstagram.Text}");
            }
            if (!string.IsNullOrWhiteSpace(txtYouTube.Text))
            {
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=youtube:{txtYouTube.Text}");
            }

            sb.AppendLine("END:VCARD");

            try
            {
                File.WriteAllText(path, sb.ToString());
                isModified = false;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Kan bestand niet opslaan: {ex.Message}", "FOUT",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Geen toegang tot bestand: {ex.Message}", "FOUT",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Onverwachte fout: {ex.Message}", "FOUT",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (isModified)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Er zijn niet-opgeslagen wijzigingen. Wil je doorgaan?",
                    "Nieuwe kaart",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            // Persoonlijk
            txtFirstname.Text = "";
            txtLastname.Text = "";
            datBirthday.SelectedDate = null;
            rdbMan.IsChecked = false;
            rdbVrouw.IsChecked = false;
            rdbOnbekend.IsChecked = false;
            txtEmail.Text = "";
            txtPhone.Text = "";
            imgFoto.Source = null;
            lblGeenSelectie.Content = "(geen geselecteerd)";

            // Werk
            txtBedrijf.Text = "";
            txtJobTitel.Text = "";
            txtWerkEmail.Text = "";
            txtWerkTelefoon.Text = "";

            // Sociaal
            txtLinkedIn.Text = "";
            txtFacebook.Text = "";
            txtInstagram.Text = "";
            txtYouTube.Text = "";

            // Statusbar resetten
            currentFilePath = null;
            mimSave.IsEnabled = false;
            txtKaart.Text = "huidige kaart: (geen geopend)";
            txtPercentage.Text = "percentage ingevuld: n.a.";

            isModified = false;
        }

        private void btnSelecteer_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Afbeeldingen (*.jpg;*.png)|*.jpg;*.png";

            if (dlg.ShowDialog() != true) return;

            try
            {
                lblGeenSelectie.Content = System.IO.Path.GetFileName(dlg.FileName);
                BitmapImage bitmap = new BitmapImage(new Uri(dlg.FileName));
                imgFoto.Source = bitmap;
                isModified = true;
                UpdatePercentage();
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show($"Bestandsformaat niet ondersteund: {ex.Message}", "FOUT", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het laden van de afbeelding: {ex.Message}", "FOUT", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePercentage()
        {
            int totalFields = 14;
            int filledFields = 0;

            if (!string.IsNullOrWhiteSpace(txtFirstname.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtLastname.Text)) filledFields++;
            if (datBirthday.SelectedDate.HasValue) filledFields++;
            if (rdbMan.IsChecked == true || rdbVrouw.IsChecked == true) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtEmail.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtPhone.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtBedrijf.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtJobTitel.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtWerkEmail.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtWerkTelefoon.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtLinkedIn.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtFacebook.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtInstagram.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtYouTube.Text)) filledFields++;

            double percentage = (filledFields / (double)totalFields) * 100;
            txtPercentage.Text = $"percentage ingevuld: {percentage:F0}%";
        }

        private void Field_Changed(object sender, EventArgs e)
        {
            isModified = true;
            UpdatePercentage();
        }

        // AI: base64 string tonen in WPF Image control
        private void LoadPhotoFromBase64(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                imgFoto.Source = null;
                lblGeenSelectie.Content = "(geen geselecteerd)";
                return;
            }
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    imgFoto.Source = bitmap;
                    lblGeenSelectie.Content = "(afbeelding geladen)";
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Ongeldige base64 afbeelding: {ex.Message}", "FOUT",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                imgFoto.Source = null;
                lblGeenSelectie.Content = "(geen geselecteerd)";
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Kan afbeelding niet lezen: {ex.Message}", "FOUT",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                imgFoto.Source = null;
                lblGeenSelectie.Content = "(geen geselecteerd)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Onverwachte fout bij laden foto: {ex.Message}", "FOUT",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                imgFoto.Source = null;
                lblGeenSelectie.Content = "(geen geselecteerd)";
            }
        }
    }
}