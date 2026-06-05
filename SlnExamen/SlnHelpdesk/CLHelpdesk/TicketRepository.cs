using System;
using System.Collections.Generic;
using System.IO;

namespace CLHelpdesk
{
    /// <summary>
    /// Beheert het lezen en schrijven van tickets via het CSV-bestand.
    /// Alle bestandslogica zit uitsluitend in deze klasse.
    /// </summary>
    public static class TicketRepository
    {
        // pad naar het CSV-bestand, ingesteld vanuit de WPF-app
        private static string _csvPad = string.Empty;

        /// <summary>
        /// Stel het pad naar het CSV-bestand in (eenmalig bij opstarten).
        /// </summary>
        public static void StelPadIn(string pad)
        {
            _csvPad = pad;
        }

        /// <summary>
        /// Lees alle tickets uit het CSV-bestand en geef ze terug als lijst.
        /// </summary>
        public static List<Ticket> GetAll()
        {
            // lege lijst aanmaken voor de resultaten
            List<Ticket> tickets = new List<Ticket>();

            // bestand controleren
            if (!File.Exists(_csvPad))
            {
                return tickets;
            }

            // alle regels inlezen
            string[] regels = File.ReadAllLines(_csvPad);

            // regels verwerken, headerregel overslaan
            for (int i = 1; i < regels.Length; i++)
            {
                string regel = regels[i].Trim();

                // lege regels overslaan
                if (string.IsNullOrEmpty(regel))
                {
                    continue;
                }

                // kolommen splitsen op puntkomma
                string[] delen = regel.Split(';');

                // regels met te weinig kolommen overslaan
                if (delen.Length < 10)
                {
                    continue;
                }

                // basisgegevens uitlezen
                int id = Convert.ToInt32(delen[0]);
                string titel = delen[1];
                string melderVoornaam = delen[2];
                string melderAchternaam = delen[3];
                string melder = $"{melderVoornaam} {melderAchternaam}";

                // prioriteit parsen via Enum.TryParse
                Enum.TryParse(delen[5], out TicketPrioriteit prioriteit);

                // status en datums verwerken
                bool isAfgesloten = Convert.ToBoolean(delen[6]);
                string type = delen[7];
                string extraInfo = delen[8];

                // datum aangemaakt parsen
                string datumStr = delen[9].Trim();
                DateTime datumAangemaakt = ParseDatum(datumStr);

                // datum afgesloten parsen als aanwezig
                DateTime? datumAfgesloten = null;
                if (delen.Length > 10 && !string.IsNullOrWhiteSpace(delen[10]))
                {
                    datumAfgesloten = ParseDatum(delen[10].Trim());
                }

                // juist ticketobject aanmaken op basis van type
                Ticket ticket;
                if (type == "Hardware")
                {
                    ticket = new HardwareTicket(id, titel, melder, prioriteit, datumAangemaakt, extraInfo);
                }
                else
                {
                    ticket = new SoftwareTicket(id, titel, melder, prioriteit, datumAangemaakt, extraInfo);
                }

                // status instellen en toevoegen aan de lijst
                ticket.IsAfgesloten = isAfgesloten;
                ticket.DatumAfgesloten = datumAfgesloten;
                tickets.Add(ticket);
            }

            // volledige lijst teruggeven
            return tickets;
        }

        /// <summary>
        /// Voeg een nieuw ticket toe aan het CSV-bestand.
        /// </summary>
        public static void VoegToe(Ticket ticket)
        {
            // type en extra info bepalen op basis van het tickettype
            string type;
            string extraInfo;

            if (ticket is HardwareTicket)
            {
                HardwareTicket hwTicket = ticket as HardwareTicket;
                type = "Hardware";
                extraInfo = hwTicket.Toestel;
            }
            else
            {
                SoftwareTicket swTicket = ticket as SoftwareTicket;
                type = "Software";
                extraInfo = swTicket.Applicatie;
            }

            // melder opsplitsen in voor- en achternaam
            string melder = ticket.Melder;
            string[] melderDelen = melder.Split(' ');
            string voornaam = melderDelen.Length > 0 ? melderDelen[0] : melder;
            string achternaam = melderDelen.Length > 1 ? melderDelen[1] : "";

            // melder-ID samenstellen uit initiaal + achternaam
            string eersteLetter = voornaam.Length > 0 ? voornaam[0].ToString().ToLower() : "";
            string melderId = eersteLetter + achternaam.ToLower();

            // datums opmaken voor het CSV-formaat
            string datumAangemaakt = ticket.DatumAangemaakt.ToString("yyyy-MM-dd HHmm");
            string datumAfgesloten = ticket.DatumAfgesloten.HasValue
                ? ticket.DatumAfgesloten.Value.ToString("yyyy-MM-dd HHmm")
                : "";

            // CSV-regel samenstellen en toevoegen aan het bestand
            string csvRegel = $"{ticket.Id};{ticket.Titel};{voornaam};{achternaam};{melderId};" +
                              $"{ticket.Prioriteit};{ticket.IsAfgesloten.ToString().ToLower()};" +
                              $"{type};{extraInfo};{datumAangemaakt};{datumAfgesloten}";

            File.AppendAllText(_csvPad, Environment.NewLine + csvRegel);
        }

        /// <summary>
        /// Sluit het ticket met het opgegeven id af in het CSV-bestand.
        /// </summary>
        public static void SluitAf(int ticketId)
        {
            // bestand controleren
            if (!File.Exists(_csvPad))
            {
                return;
            }

            // alle regels inlezen
            string[] regels = File.ReadAllLines(_csvPad);

            // vlag om bij te houden of het ticket gevonden is
            bool gevonden = false;

            // regels doorlopen op zoek naar het juiste ticket
            for (int i = 1; i < regels.Length; i++)
            {
                // overige regels overslaan als ticket al gevonden is
                if (gevonden)
                {
                    continue;
                }

                string[] delen = regels[i].Split(';');

                // te korte regels overslaan
                if (delen.Length < 10)
                {
                    continue;
                }

                // juiste rij controleren via ticket-ID
                if (Convert.ToInt32(delen[0]) == ticketId)
                {
                    // isAfgesloten op true zetten
                    delen[6] = "true";

                    // datumAfgesloten instellen
                    if (delen.Length > 10)
                    {
                        delen[10] = DateTime.Now.ToString("yyyy-MM-dd HHmm");
                    }
                    else
                    {
                        // kolom toevoegen als die ontbreekt
                        string[] uitgebreideDelen = new string[11];
                        for (int j = 0; j < delen.Length; j++)
                        {
                            uitgebreideDelen[j] = delen[j];
                        }
                        uitgebreideDelen[10] = DateTime.Now.ToString("yyyy-MM-dd HHmm");
                        delen = uitgebreideDelen;
                    }

                    // bijgewerkte regel terugzetten
                    regels[i] = string.Join(";", delen);
                    gevonden = true;
                }
            }

            // bijgewerkte regels terugschrijven naar het bestand
            File.WriteAllLines(_csvPad, regels);
        }

        /// <summary>
        /// Geeft het volgende beschikbare ticket-ID terug.
        /// </summary>
        public static int VolgendeId()
        {
            // alle tickets ophalen
            List<Ticket> alle = GetAll();

            // als er geen tickets zijn, beginnen we bij 1
            if (alle.Count == 0)
            {
                return 1;
            }

            // hoogste ID zoeken
            int max = 0;
            foreach (Ticket t in alle)
            {
                if (t.Id > max)
                {
                    max = t.Id;
                }
            }

            // volgend ID teruggeven
            return max + 1;
        }

        /// <summary>
        /// Hulpmethode: parst een datum uit het formaat "yyyy-MM-dd HHmm".
        /// </summary>
        private static DateTime ParseDatum(string datumStr)
        {
            // verwacht formaat: "yyyy-MM-dd HHmm" = 15 tekens
            if (datumStr.Length == 15)
            {
                string jaar = datumStr.Substring(0, 4);
                string maand = datumStr.Substring(5, 2);
                string dag = datumStr.Substring(8, 2);
                string uur = datumStr.Substring(11, 2);
                string minuut = datumStr.Substring(13, 2);

                return new DateTime(
                    Convert.ToInt32(jaar),
                    Convert.ToInt32(maand),
                    Convert.ToInt32(dag),
                    Convert.ToInt32(uur),
                    Convert.ToInt32(minuut),
                    0);
            }

            // terugval: standaard DateTime.TryParse proberen
            DateTime dt;
            if (DateTime.TryParse(datumStr, out dt))
            {
                return dt;
            }

            return DateTime.Now;
        }
    }
}
