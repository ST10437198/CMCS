using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using CMCS.Models;

namespace CMCS.Data
{
    public static class DatabaseHelper
    {
        // UPDATE this connection string to your server/database.
        private static string _connectionString = "Server=.;Database=ClaimsAppDb;Trusted_Connection=True;TrustServerCertificate=True;";


        public static async Task<User> GetUserByUsernameAsync(string username)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT UserId, Username, Password, Role FROM [User] WHERE Username = @u";
                cmd.Parameters.AddWithValue("@u", username);
                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    if (await rdr.ReadAsync())
                    {
                        return new User
                        {
                            UserId = rdr.GetInt32(0),
                            Username = rdr.GetString(1),
                            Password = rdr.GetString(2),
                            Role = rdr.GetString(3)
                        };
                    }
                }
            }

            return null;
        }

        public static async Task<int> InsertClaimAsync(ClaimModel claim)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO Claim (UserId, HoursWorked, HourlyRate, Notes, AttachmentPath)
                    VALUES (@uid, @hrs, @rate, @notes, @attach);
                    SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@uid", claim.UserId);
                cmd.Parameters.AddWithValue("@hrs", claim.HoursWorked);
                cmd.Parameters.AddWithValue("@rate", claim.HourlyRate);
                cmd.Parameters.AddWithValue("@notes", (object)claim.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@attach", (object)claim.AttachmentPath ?? DBNull.Value);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        public static async Task<List<ClaimModel>> GetPendingClaimsAsync()
        {
            var list = new List<ClaimModel>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT c.ClaimId, c.UserId, c.DateSubmitted, c.HoursWorked, c.HourlyRate, c.Notes, c.AttachmentPath, c.Status, u.Username
                    FROM Claim c
                    INNER JOIN [User] u ON c.UserId = u.UserId
                    WHERE c.Status = 'Pending'
                    ORDER BY c.DateSubmitted DESC";
                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        list.Add(new ClaimModel
                        {
                            ClaimId = rdr.GetInt32(0),
                            UserId = rdr.GetInt32(1),
                            DateSubmitted = rdr.GetDateTime(2),
                            HoursWorked = rdr.GetDecimal(3),
                            HourlyRate = rdr.GetDecimal(4),
                            Notes = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                            AttachmentPath = rdr.IsDBNull(6) ? null : rdr.GetString(6),
                            Status = rdr.GetString(7),
                            SubmitterUsername = rdr.GetString(8)
                        });
                    }
                }
            }

            return list;
        }

        public static async Task<List<ClaimModel>> GetAllClaimsAsync()
        {
            var list = new List<ClaimModel>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
            SELECT c.ClaimId, c.UserId, c.DateSubmitted, c.HoursWorked, c.HourlyRate, 
                   c.Notes, c.AttachmentPath, c.Status, u.Username
            FROM Claim c
            INNER JOIN [User] u ON c.UserId = u.UserId
            ORDER BY c.DateSubmitted DESC"; // ✅ NO STATUS FILTER

                await conn.OpenAsync();

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        list.Add(new ClaimModel
                        {
                            ClaimId = rdr.GetInt32(0),
                            UserId = rdr.GetInt32(1),
                            DateSubmitted = rdr.GetDateTime(2),
                            HoursWorked = rdr.GetDecimal(3),
                            HourlyRate = rdr.GetDecimal(4),
                            Notes = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                            AttachmentPath = rdr.IsDBNull(6) ? null : rdr.GetString(6),
                            Status = rdr.GetString(7),
                            SubmitterUsername = rdr.GetString(8)
                        });
                    }
                }
            }

            return list;
        }


        public static async Task UpdateClaimStatusAsync(int claimId, string status, int reviewerId)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    UPDATE Claim
                    SET Status = @status, ReviewedBy = @rev, ReviewedOn = GETDATE()
                    WHERE ClaimId = @cid";
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@rev", reviewerId);
                cmd.Parameters.AddWithValue("@cid", claimId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}

