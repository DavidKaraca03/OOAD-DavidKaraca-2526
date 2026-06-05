using System;

namespace CLHelpdesk
{
    /// <summary>
    /// Ticket voor hardware-gerelateerde problemen.
    /// </summary>
    public class HardwareTicket : Ticket
    {
        // Welk toestel of apparaat het probleem heeft
        public string Toestel { get; set; }

        /// <summary>
        /// Lege constructor voor flexibele aanmaak.
        /// </summary>
        public HardwareTicket() { }

        /// <summary>
        /// Constructor met alle gegevens inclusief toestelomschrijving.
        /// </summary>
        public HardwareTicket(int id, string titel, string melder, TicketPrioriteit prioriteit,
            DateTime datumAangemaakt, string toestel)
            : base(id, titel, melder, prioriteit, datumAangemaakt)
        {
            Toestel = toestel;
        }

        /// <summary>
        /// Geeft uitgebreide detailinformatie terug inclusief het betrokken toestel.
        /// </summary>
        public override string GeefInfo()
        {
            // basisinfo aanvullen met hardware-specifieke gegevens
            return base.GeefInfo() + "\n" +
                   $"Type:        Hardware\n" +
                   $"Toestel:     {Toestel}";
        }
    }
}
