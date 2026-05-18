using System;

namespace Lib
{
    /// <summary>
    /// Stelt een patiënt voor in de dokterspraktijk.
    /// Erft van Persoon.
    /// </summary>
    public class Patient : Persoon
    {
        // Extra properties specifiek voor een patiënt
        public GeslachtType Geslacht { get; set; }
        public DateTime Geboortedatum { get; set; }
        public NotificatieType Notificaties { get; set; }

        // Standaard constructor
        public Patient() { }

        // Constructor met naam en e-mail
        public Patient(string voornaam, string achternaam, string email)
            : base(voornaam, achternaam, email) { }

        /// <summary>
        /// Geeft de volledige naam van de patiënt terug.
        /// </summary>
        public override string ToString()
        {
            return $"{Voornaam} {Achternaam}";
        }
    }
}
