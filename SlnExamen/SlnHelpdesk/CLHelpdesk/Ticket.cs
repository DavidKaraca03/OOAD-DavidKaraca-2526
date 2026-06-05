using System;

namespace CLHelpdesk
{
    /// <summary>
    /// Abstracte basisklasse voor alle helpdesk tickets.
    /// </summary>
    public abstract class Ticket
    {
        // Eigenschappen van een ticket
        public int Id { get; set; }
        public string Titel { get; set; }
        public string Melder { get; set; }
        public TicketPrioriteit Prioriteit { get; set; }
        public bool IsAfgesloten { get; set; } = false;
        public DateTime DatumAangemaakt { get; set; }
        public DateTime? DatumAfgesloten { get; set; }

        /// <summary>
        /// Lege constructor voor flexibele aanmaak.
        /// </summary>
        public Ticket() { }

        /// <summary>
        /// Constructor met alle basisgegevens.
        /// </summary>
        public Ticket(int id, string titel, string melder, TicketPrioriteit prioriteit, DateTime datumAangemaakt)
        {
            Id = id;
            Titel = titel;
            Melder = melder;
            Prioriteit = prioriteit;
            DatumAangemaakt = datumAangemaakt;
        }

        /// <summary>
        /// Geeft een beknopte omschrijving terug voor gebruik in de ListBox.
        /// </summary>
        public override string ToString()
        {
            // status bepalen op basis van IsAfgesloten
            string status = IsAfgesloten ? "[GESLOTEN]" : "[OPEN]";
            return $"{status} #{Id} – {Titel} ({Prioriteit})";
        }

        /// <summary>
        /// Geeft uitgebreide detailinformatie terug. Wordt overschreven in subklassen.
        /// </summary>
        public virtual string GeefInfo()
        {
            // status en datum afgesloten bepalen
            string status = IsAfgesloten ? "Afgesloten" : "Open";
            string afgesloten = DatumAfgesloten.HasValue
                ? DatumAfgesloten.Value.ToString("dd/MM/yyyy HH:mm")
                : "–";

            // opgemaakte tekst samenstellen
            return $"Titel:       {Titel}\n" +
                   $"Melder:      {Melder}\n" +
                   $"Prioriteit:  {Prioriteit}\n" +
                   $"Status:      {status}\n" +
                   $"Aangemaakt:  {DatumAangemaakt:dd/MM/yyyy HH:mm}\n" +
                   $"Afgesloten:  {afgesloten}";
        }
    }
}
