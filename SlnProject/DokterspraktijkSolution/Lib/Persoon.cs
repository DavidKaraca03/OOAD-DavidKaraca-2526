using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Lib
{
    /// <summary>
    /// Abstracte superklasse voor Patient en Dokter.
    /// Bevat gemeenschappelijke properties en de SHA256-hashfunctie.
    /// </summary>
    public abstract class Persoon
    {
        // Verbindingsstring uit App.config, gedeeld met subklassen
        protected static string _connString =
            ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        // Gemeenschappelijke properties
        public int Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Gsm { get; set; }
        public string Email { get; set; }
        public string Paswoord { get; set; }
        public byte[] Profielfotodata { get; set; }

        // Standaard constructor
        public Persoon() { }

        // Constructor met naam en e-mail
        public Persoon(string voornaam, string achternaam, string email)
        {
            Voornaam = voornaam;
            Achternaam = achternaam;
            Email = email;
        }

        /// <summary>
        /// Hasht een wachtwoord met SHA256 en geeft de hexadecimale string terug.
        /// </summary>
        public static string HashPaswoord(string paswoord)
        {
            // SHA256-object aanmaken en bytes berekenen
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(paswoord));

                // bytes omzetten naar een hexadecimale string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Geeft de volledige naam terug.
        /// </summary>
        public override string ToString()
        {
            return $"{Voornaam} {Achternaam}";
        }
    }
}
