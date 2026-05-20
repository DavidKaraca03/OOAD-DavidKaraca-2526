using System;
using System.Collections.Generic;
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
                            d.Id                 = Convert.ToInt32(reader["Id"]);
                            d.Voornaam           = reader["Voornaam"].ToString();
                            d.Achternaam         = reader["Achternaam"].ToString();
                            d.Email              = reader["Email"].ToString();
                            d.Gsm                = reader["Gsm"].ToString();
                            d.Rizivnummer        = Convert.ToInt32(reader["Rizivnummer"]);
                            d.IsGeconventioneerd = Convert.ToBoolean(reader["IsGeconventioneerd"]);
                            return d;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Haalt alle dokters op uit de database, gesorteerd op achternaam.
        /// </summary>
        public static List<Dokter> GetAll()
        {
            // lijst aanmaken om resultaten in op te slaan
            List<Dokter> dokters = new List<Dokter>();

            // verbinding openen en query uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT id, voornaam, achternaam, gsm, email, " +
                             "rizivnummer, isgeconventioneerd, profielfotodata " +
                             "FROM Dokter ORDER BY achternaam, voornaam";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                // resultaten inlezen en omzetten naar Dokter-objecten
                while (reader.Read())
                {
                    dokters.Add(VulDokterIn(reader));
                }
            }

            // resultaat teruggeven
            return dokters;
        }

        /// <summary>
        /// Haalt één dokter op via het id, of null als niet gevonden.
        /// </summary>
        public static Dokter GetById(int id)
        {
            // verbinding openen en query uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT id, voornaam, achternaam, gsm, email, " +
                             "rizivnummer, isgeconventioneerd, profielfotodata " +
                             "FROM Dokter WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                // één resultaat teruggeven indien gevonden
                if (reader.Read())
                {
                    return VulDokterIn(reader);
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

        /// <summary>
        /// Vult een Dokter-object in vanuit een SqlDataReader.
        /// </summary>
        private static Dokter VulDokterIn(SqlDataReader reader)
        {
            Dokter d = new Dokter();
            d.Id                 = Convert.ToInt32(reader["id"]);
            d.Voornaam           = reader["voornaam"].ToString();
            d.Achternaam         = reader["achternaam"].ToString();
            d.Gsm                = reader["gsm"].ToString();
            d.Email              = reader["email"].ToString();
            d.Rizivnummer        = Convert.ToInt32(reader["rizivnummer"]);
            d.IsGeconventioneerd = Convert.ToBoolean(reader["isgeconventioneerd"]);

            // foto is nullable, controleer op DBNull
            d.Profielfotodata = reader["profielfotodata"] == DBNull.Value
                                    ? null
                                    : (byte[])reader["profielfotodata"];
            return d;
        }
    }
}
