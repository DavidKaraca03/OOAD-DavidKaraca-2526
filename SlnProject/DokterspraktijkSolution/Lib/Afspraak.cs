using System;

namespace Lib
{
    /// <summary>
    /// Stelt een afspraak voor tussen een patiënt en een dokter.
    /// </summary>
    public class Afspraak
    {
        

        // Properties
        public int Id { get; set; }
        public DateTime Moment { get; set; }
        public string Klacht { get; set; }
        public int PatientId { get; set; }
        public int DokterId { get; set; }

        // Standaard constructor
        public Afspraak() { }

        /// <summary>
        /// Geeft datum, tijdstip en klacht van de afspraak terug.
        /// </summary>
        public override string ToString()
        {
            return $"{Moment:dd/MM/yyyy HH:mm} - {Klacht}";
        }
    }
}
