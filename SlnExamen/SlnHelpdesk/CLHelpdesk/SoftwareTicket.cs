using System;

namespace CLHelpdesk
{
    /// <summary>
    /// Ticket voor software-gerelateerde problemen.
    /// </summary>
    public class SoftwareTicket : Ticket
    {
        // Welke applicatie of software het probleem heeft
        public string Applicatie { get; set; }

        /// <summary>
        /// Lege constructor voor flexibele aanmaak.
        /// </summary>
        public SoftwareTicket() { }

        /// <summary>
        /// Constructor met alle gegevens inclusief applicatienaam.
        /// </summary>
        public SoftwareTicket(int id, string titel, string melder, TicketPrioriteit prioriteit,
            DateTime datumAangemaakt, string applicatie)
            : base(id, titel, melder, prioriteit, datumAangemaakt)
        {
            Applicatie = applicatie;
        }

        /// <summary>
        /// Geeft uitgebreide detailinformatie terug inclusief de betrokken applicatie.
        /// </summary>
        public override string GeefInfo()
        {
            // basisinfo aanvullen met software-specifieke gegevens
            return base.GeefInfo() + "\n" +
                   $"Type:        Software\n" +
                   $"Applicatie:  {Applicatie}";
        }
    }
}
