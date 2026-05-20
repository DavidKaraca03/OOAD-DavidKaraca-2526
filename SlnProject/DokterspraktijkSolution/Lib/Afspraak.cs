using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Lib
{
    /// <summary>
    /// Stelt een afspraak voor tussen een patiënt en een dokter.
    /// </summary>
    public class Afspraak
    {
        // Verbindingsstring uit App.config
        private static string _connString =
            ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        // Properties
        public int Id { get; set; }
        public DateTime Moment { get; set; }
        public string Klacht { get; set; }
        public int PatientId { get; set; }
        public int DokterId { get; set; }

        /// <summary>
        /// Gelinkte patiënt (read-only, haalt telkens op uit de database).
        /// </summary>
        public Patient Patient
        {
            get { return Lib.Patient.GetById(PatientId); }
        }

        /// <summary>
        /// Gelinkte dokter (read-only, haalt telkens op uit de database).
        /// </summary>
        public Dokter Dokter
        {
            get { return Lib.Dokter.GetById(DokterId); }
        }

        // Standaard constructor
        public Afspraak() { }

        /// <summary>
        /// Haalt alle afspraken op voor een dokter op een bepaalde datum, gesorteerd op tijdstip.
        /// </summary>
        public static List<Afspraak> GetByDokter(int dokterId, DateTime datum)
        {
            // lijst aanmaken om resultaten in op te slaan
            List<Afspraak> afspraken = new List<Afspraak>();

            // verbinding openen en query uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT id, moment, klacht, patient_id, dokter_id " +
                             "FROM Afspraak " +
                             "WHERE dokter_id = @dokterId " +
                             "AND CAST(moment AS DATE) = CAST(@datum AS DATE) " +
                             "ORDER BY moment";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@dokterId", dokterId);
                cmd.Parameters.AddWithValue("@datum", datum.Date);
                SqlDataReader reader = cmd.ExecuteReader();

                // resultaten inlezen en omzetten naar Afspraak-objecten
                while (reader.Read())
                {
                    Afspraak a = new Afspraak();
                    a.Id        = Convert.ToInt32(reader["id"]);
                    a.Moment    = Convert.ToDateTime(reader["moment"]);
                    a.Klacht    = reader["klacht"].ToString();
                    a.PatientId = Convert.ToInt32(reader["patient_id"]);
                    a.DokterId  = Convert.ToInt32(reader["dokter_id"]);
                    afspraken.Add(a);
                }
            }

            // resultaat teruggeven
            return afspraken;
        }

        /// <summary>
        /// Haalt alle afspraken op voor een patiënt, gesorteerd op tijdstip.
        /// </summary>
        public static List<Afspraak> GetByPatient(int patientId)
        {
            // lijst aanmaken om resultaten in op te slaan
            List<Afspraak> afspraken = new List<Afspraak>();

            // verbinding openen en query uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT id, moment, klacht, patient_id, dokter_id " +
                             "FROM Afspraak " +
                             "WHERE patient_id = @patientId " +
                             "ORDER BY moment";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@patientId", patientId);
                SqlDataReader reader = cmd.ExecuteReader();

                // resultaten inlezen
                while (reader.Read())
                {
                    Afspraak a = new Afspraak();
                    a.Id        = Convert.ToInt32(reader["id"]);
                    a.Moment    = Convert.ToDateTime(reader["moment"]);
                    a.Klacht    = reader["klacht"].ToString();
                    a.PatientId = Convert.ToInt32(reader["patient_id"]);
                    a.DokterId  = Convert.ToInt32(reader["dokter_id"]);
                    afspraken.Add(a);
                }
            }

            // resultaat teruggeven
            return afspraken;
        }

        /// <summary>
        /// Voegt de afspraak in de database in en geeft het nieuwe id terug.
        /// </summary>
        public int InsertInDb()
        {
            // verbinding openen en insert uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "INSERT INTO Afspraak (moment, klacht, patient_id, dokter_id) " +
                             "VALUES (@moment, @klacht, @patientId, @dokterId); " +
                             "SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@moment", Moment);
                cmd.Parameters.AddWithValue("@klacht", Klacht);
                cmd.Parameters.AddWithValue("@patientId", PatientId);
                cmd.Parameters.AddWithValue("@dokterId", DokterId);

                // nieuw id ophalen en bijhouden
                Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return Id;
        }

        /// <summary>
        /// Verwijdert de afspraak uit de database.
        /// </summary>
        public void DeleteFromDb()
        {
            // verbinding openen en delete uitvoeren
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "DELETE FROM Afspraak WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Geeft datum, tijdstip en klacht van de afspraak terug.
        /// </summary>
        public override string ToString()
        {
            return $"{Moment:dd/MM/yyyy HH:mm} - {Klacht}";
        }
    }
}
