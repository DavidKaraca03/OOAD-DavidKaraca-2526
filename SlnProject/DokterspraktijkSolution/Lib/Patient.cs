using System;
using System.Collections.Generic;
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
                            p.Id            = Convert.ToInt32(reader["Id"]);
                            p.Voornaam      = reader["Voornaam"].ToString();
                            p.Achternaam    = reader["Achternaam"].ToString();
                            p.Email         = reader["Email"].ToString();
                            p.Gsm           = reader["Gsm"].ToString();
                            p.Geslacht      = (GeslachtType)Convert.ToInt32(reader["Geslacht"]);
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
        /// Haalt alle patiënten op uit de database, gesorteerd op achternaam.
        /// </summary>
        public static List<Patient> GetAll()
        {
            // lijst aanmaken om resultaten in op te slaan
            List<Patient> patienten = new List<Patient>();

            // verbinding openen en query uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, " +
                             "geboortedatum, notificaties, profielfotodata " +
                             "FROM Patient ORDER BY achternaam, voornaam";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                // resultaten inlezen en omzetten naar Patient-objecten
                while (reader.Read())
                {
                    patienten.Add(VulPatientIn(reader));
                }
            }

            // resultaat teruggeven
            return patienten;
        }

        /// <summary>
        /// Haalt patiënten op gefilterd op naam (voornaam of achternaam).
        /// </summary>
        public static List<Patient> GetAll(string zoekterm)
        {
            // lijst aanmaken om resultaten in op te slaan
            List<Patient> patienten = new List<Patient>();

            // verbinding openen en gefilterde query uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, " +
                             "geboortedatum, notificaties, profielfotodata " +
                             "FROM Patient " +
                             "WHERE voornaam LIKE @zoekterm OR achternaam LIKE @zoekterm " +
                             "ORDER BY achternaam, voornaam";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@zoekterm", "%" + zoekterm + "%");
                SqlDataReader reader = cmd.ExecuteReader();

                // resultaten inlezen
                while (reader.Read())
                {
                    patienten.Add(VulPatientIn(reader));
                }
            }

            // resultaat teruggeven
            return patienten;
        }

        /// <summary>
        /// Haalt één patiënt op via het id, of null als niet gevonden.
        /// </summary>
        public static Patient GetById(int id)
        {
            // verbinding openen en query uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, " +
                             "geboortedatum, notificaties, profielfotodata " +
                             "FROM Patient WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                // één resultaat teruggeven indien gevonden
                if (reader.Read())
                {
                    return VulPatientIn(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// Voegt de patiënt in de database in en geeft het nieuwe id terug.
        /// </summary>
        public int InsertInDb()
        {
            // verbinding openen en insert uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "INSERT INTO Patient " +
                             "(voornaam, achternaam, geslacht, gsm, email, paswoord, " +
                             "geboortedatum, notificaties, profielfotodata) " +
                             "VALUES (@voornaam, @achternaam, @geslacht, @gsm, @email, @paswoord, " +
                             "@geboortedatum, @notificaties, @profielfotodata); " +
                             "SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@voornaam", Voornaam);
                cmd.Parameters.AddWithValue("@achternaam", Achternaam);
                cmd.Parameters.AddWithValue("@geslacht", (int)Geslacht);
                cmd.Parameters.AddWithValue("@gsm", Gsm != null ? (object)Gsm : DBNull.Value);
                cmd.Parameters.AddWithValue("@email", Email);
                cmd.Parameters.AddWithValue("@paswoord", Paswoord);
                cmd.Parameters.AddWithValue("@geboortedatum", Geboortedatum);
                cmd.Parameters.AddWithValue("@notificaties", (int)Notificaties);
                cmd.Parameters.AddWithValue("@profielfotodata",
                    Profielfotodata != null ? (object)Profielfotodata : DBNull.Value);

                // nieuw id ophalen en bijhouden
                Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return Id;
        }

        /// <summary>
        /// Werkt de patiëntgegevens bij in de database (wachtwoord wordt niet gewijzigd).
        /// </summary>
        public void UpdateInDb()
        {
            // verbinding openen en update uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "UPDATE Patient SET " +
                             "voornaam = @voornaam, achternaam = @achternaam, " +
                             "geslacht = @geslacht, gsm = @gsm, email = @email, " +
                             "geboortedatum = @geboortedatum, notificaties = @notificaties, " +
                             "profielfotodata = @profielfotodata " +
                             "WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@voornaam", Voornaam);
                cmd.Parameters.AddWithValue("@achternaam", Achternaam);
                cmd.Parameters.AddWithValue("@geslacht", (int)Geslacht);
                cmd.Parameters.AddWithValue("@gsm", Gsm != null ? (object)Gsm : DBNull.Value);
                cmd.Parameters.AddWithValue("@email", Email);
                cmd.Parameters.AddWithValue("@geboortedatum", Geboortedatum);
                cmd.Parameters.AddWithValue("@notificaties", (int)Notificaties);
                cmd.Parameters.AddWithValue("@profielfotodata",
                    Profielfotodata != null ? (object)Profielfotodata : DBNull.Value);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Verwijdert de patiënt en alle gekoppelde afspraken uit de database.
        /// </summary>
        public void DeleteFromDb()
        {
            // verbinding openen
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();

                // eerst gekoppelde afspraken verwijderen (foreign key)
                string sqlAfspraken = "DELETE FROM Afspraak WHERE patient_id = @id";
                SqlCommand cmdAfspraken = new SqlCommand(sqlAfspraken, conn);
                cmdAfspraken.Parameters.AddWithValue("@id", Id);
                cmdAfspraken.ExecuteNonQuery();

                // daarna de patiënt zelf verwijderen
                string sqlPatient = "DELETE FROM Patient WHERE id = @id";
                SqlCommand cmdPatient = new SqlCommand(sqlPatient, conn);
                cmdPatient.Parameters.AddWithValue("@id", Id);
                cmdPatient.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Geeft de volledige naam van de patiënt terug.
        /// </summary>
        public override string ToString()
        {
            return $"{Voornaam} {Achternaam}";
        }

        /// <summary>
        /// Vult een Patient-object in vanuit een SqlDataReader.
        /// </summary>
        private static Patient VulPatientIn(SqlDataReader reader)
        {
            Patient p = new Patient();
            p.Id              = Convert.ToInt32(reader["id"]);
            p.Voornaam        = reader["voornaam"].ToString();
            p.Achternaam      = reader["achternaam"].ToString();
            p.Gsm             = reader["gsm"].ToString();
            p.Email           = reader["email"].ToString();
            p.Geslacht        = (GeslachtType)Convert.ToInt32(reader["geslacht"]);
            p.Geboortedatum   = Convert.ToDateTime(reader["geboortedatum"]);
            p.Notificaties    = (NotificatieType)Convert.ToInt32(reader["notificaties"]);

            // foto is nullable, controleer op DBNull
            p.Profielfotodata = reader["profielfotodata"] == DBNull.Value
                                    ? null
                                    : (byte[])reader["profielfotodata"];
            return p;
        }
    }
}
