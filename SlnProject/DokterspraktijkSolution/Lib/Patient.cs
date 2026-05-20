using System;
using Microsoft.Data.SqlClient;

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
        /// Controleert e-mail en wachtwoord in de database en geeft de patiënt terug, of null bij mislukking.
        /// </summary>
        public static Patient Login(string email, string paswoord)
        {
            string hash = HashPaswoord(paswoord);
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT Id, Voornaam, Achternaam, Email, Gsm, Geslacht, Geboortedatum, Notificaties " +
                             "FROM Patient WHERE Email = @email AND Paswoord = @paswoord";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@paswoord", hash);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Patient p = new Patient();
                            p.Id          = Convert.ToInt32(reader["Id"]);
                            p.Voornaam    = reader["Voornaam"].ToString();
                            p.Achternaam  = reader["Achternaam"].ToString();
                            p.Email       = reader["Email"].ToString();
                            p.Gsm         = reader["Gsm"].ToString();
                            p.Geslacht    = (GeslachtType)Convert.ToInt32(reader["Geslacht"]);
                            p.Geboortedatum = Convert.ToDateTime(reader["Geboortedatum"]);
                            p.Notificaties  = (NotificatieType)Convert.ToInt32(reader["Notificaties"]);
                            return p;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Geeft de volledige naam van de patiënt terug.
        /// </summary>
        public override string ToString()
        {
            return $"{Voornaam} {Achternaam}";
        }
    }
}
