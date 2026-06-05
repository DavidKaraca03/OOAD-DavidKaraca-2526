using System.Collections.Generic;

namespace CLHelpdesk
{
    /// <summary>
    /// Stelt een medewerker voor die tickets kan indienen.
    /// </summary>
    public class Medewerker
    {
        // Identificatie van de medewerker
        public string Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }

        // Lijst van tickets ingediend door deze medewerker
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();

        /// <summary>
        /// Lege constructor.
        /// </summary>
        public Medewerker() { }

        /// <summary>
        /// Constructor met id, voornaam en achternaam.
        /// </summary>
        public Medewerker(string id, string voornaam, string achternaam)
        {
            Id = id;
            Voornaam = voornaam;
            Achternaam = achternaam;
        }

        /// <summary>
        /// Geeft de volledige naam van de medewerker terug.
        /// </summary>
        public override string ToString()
        {
            return $"{Voornaam} {Achternaam}";
        }
    }
}
