namespace Lib
{
    /// <summary>
    /// Stelt een dokter voor in de dokterspraktijk.
    /// Erft van Persoon.
    /// </summary>
    public class Dokter : Persoon
    {
        // Extra properties specifiek voor een dokter
        public int Rizivnummer { get; set; }
        public bool IsGeconventioneerd { get; set; }

        // Standaard constructor
        public Dokter() { }

        // Constructor met naam en e-mail
        public Dokter(string voornaam, string achternaam, string email)
            : base(voornaam, achternaam, email) { }

        /// <summary>
        /// Geeft de volledige naam van de dokter terug.
        /// </summary>
        public override string ToString()
        {
            return $"{Voornaam} {Achternaam}";
        }
    }
}
