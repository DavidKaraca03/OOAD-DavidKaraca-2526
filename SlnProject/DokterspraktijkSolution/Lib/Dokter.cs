using System;
using Microsoft.Data.SqlClient;

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
        /// Controleert e-mail en wachtwoord in de database en geeft de dokter terug, of null bij mislukking.
        /// </summary>
        public static Dokter Login(string email, string paswoord)
        {
            string hash = HashPaswoord(paswoord);
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT Id, Voornaam, Achternaam, Email, Gsm, Rizivnummer, IsGeconventioneerd " +
                             "FROM Dokter WHERE Email = @email AND Paswoord = @paswoord";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@paswoord", hash);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Dokter d = new Dokter();
                            d.Id       = Convert.ToInt32(reader["Id"]);
                            d.Voornaam = reader["Voornaam"].ToString();
                            d.Achternaam = reader["Achternaam"].ToString();
                            d.Email    = reader["Email"].ToString();
                            d.Gsm      = reader["Gsm"].ToString();
                            d.Rizivnummer = Convert.ToInt32(reader["Rizivnummer"]);
                            d.IsGeconventioneerd = Convert.ToBoolean(reader["IsGeconventioneerd"]);
                            return d;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Geeft de volledige naam van de dokter terug.
        /// </summary>
        public override string ToString()
        {
            return $"{Voornaam} {Achternaam}";
        }
    }
}
