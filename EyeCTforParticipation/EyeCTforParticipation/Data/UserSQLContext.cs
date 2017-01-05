using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Device.Location;

namespace EyeCTforParticipation.Data
{
    class UserSQLContext : IUserContext
    {
        public UserModel Login(string rfid)
        {
            UserModel result = null;
            string query = @"SELECT Id, Role, Name, Approved, Zoom
                             FROM [User] 
                             WHERE RFID = @RFID;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@RFID", rfid);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Role = (UserRole)reader.GetInt32(1),
                            Name = reader.GetString(2),
                            Approved = reader.GetBoolean(3),
                            Zoom = reader.GetInt32(4)
                        };
                    }
                }
            }
            return result;
        }

        public UserModel LoginPassword(string email)
        {
            UserModel result = null;
            string query = @"SELECT Id, Role, Name, Password, Approved, Zoom, Birthdate
                             FROM [User] 
                             WHERE Email = @Email;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@Email", email);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Role = (UserRole)reader.GetInt32(1),
                            Name = reader.GetString(2),
                            Password = reader.GetString(3),
                            Approved = reader.GetBoolean(4),
                            Zoom = reader.GetInt32(5),
                            Birthdate = reader.GetDateTime(6)
                        };
                    }
                }
            }
            return result;
        }

        public int Register(UserModel user, bool approved)
        {
            int id;
            string query = @"INSERT INTO [User] 
                             (Role, Email, Name, Password, Birthdate, Approved) 
                             VALUES(@Role, @Email, @Name, @Password, @Birthdate, @Approved); 
                             SELECT SCOPE_IDENTITY();";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@Role", (int)user.Role);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Birthdate", user.Birthdate);
                cmd.Parameters.AddWithValue("@Approved", approved);
                id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return id;
        }

        public void RegisterVolunteer(VolunteerModel volunteer)
        {
            string query = @"INSERT INTO [Volunteer] 
                             (Id, Address, Location, DriversLicense, Car) 
                             VALUES(@Id, @Address, geography::STPointFromText(@Location, 4326), @DriversLicense, @Car); 
                             SELECT SCOPE_IDENTITY();";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@Id", volunteer.Id);
                cmd.Parameters.AddWithValue("@Address", volunteer.Address);
                cmd.Parameters.AddWithValue("@Location", "POINT(" + volunteer.Location.Longitude + " " + volunteer.Location.Latitude + ")");
                cmd.Parameters.AddWithValue("@DriversLicense", volunteer.DriversLicense);
                cmd.Parameters.AddWithValue("@Car", volunteer.Car);
                cmd.ExecuteNonQuery();
            }
        }

        public void ApproveRegistration(int userId)
        {
            string query = @"UPDATE [User]
                            SET Approved = 1
                            WHERE Id = @Id;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.ExecuteNonQuery();
            }
        }
        public void Profile(string name, DateTime birthdate, int userId)
        {
            string query = @"UPDATE [User] 
                             SET Name = @Name, Birthdate = @Birthdate
                             WHERE Id = @Id;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Birthdate", birthdate);
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.ExecuteNonQuery();
            }
        }
        public void Delete(int Id)
        {
            throw new NotImplementedException();
        }
        public void AddHelpSeeker(int helpSeekerId, int aidWorkerId)
        {
            string query = @"INSERT INTO [HelpSeekerAidWorker] 
                             (HelpSeekerUserId, AidWorkerUserId, Approved) 
                             VALUES(@HelpSeekerUserId, @AidWorkerUserId, 0);";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@HelpSeekerUserId", helpSeekerId);
                cmd.Parameters.AddWithValue("@AidWorkerUserId", aidWorkerId);
            }
        }
        public void RemoveHelpSeeker(int helpSeekerId, int aidWorkerId)
        {
            string query = @"DELETE FROM [HelpSeekerAidWorker] 
                             WHERE HelpSeekerUserId = @helpSeekerUserId AND AidWorkerUserId = @AidWorkerUserId;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@helpSeekerUserId", helpSeekerId);
                cmd.Parameters.AddWithValue("@AidWorkerUserId", aidWorkerId);
                cmd.ExecuteNonQuery();
            }
        }
        public void ChangeApproveAidWorker(int helpSeekerId, int aidWorkerID, bool approved)
        {
            string query = @"UPDATE [HelpSeekerAidWorker] 
                             SET Approved = @Approved 
                             WHERE HelpSeekerUserId = @helpSeekerId AND AidWorkerUserId = @aidWorkerId;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@helpSeekerId", helpSeekerId);
                cmd.Parameters.AddWithValue("@AidWorkerId", aidWorkerID);
                cmd.Parameters.AddWithValue("@Approved", approved);
                cmd.ExecuteNonQuery();
            }
        }
        public List<UserModel> GetHelpSeekers(int aidWorkerId)
        {
            List<UserModel> results = new List<UserModel>();
            string query = @"SELECT [User].Id, [User].Name
                             FROM [User]
                             WHERE User.ID IN (
                                SELECT HelpSeekerUserId
                                FROM [HelpSeekerAidWorker]
                                WHERE AidWorkerUserID = @aidWorkerId
                             );";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@aidWorkerID", aidWorkerId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                        });
                    }
                }
            }
            return results;
        }

        public List<UserModel> GetAidWorkers(int helpSeekerId)
        {
            List<UserModel> results = new List<UserModel>();
            string query = @"SELECT [User].Id, [User].Name
                             FROM [User] 
                             WHERE User.ID IN (
                                SELECT AidWorkerUserId
                                FROM [HelpSeekerAidWorker]
                                WHERE HelpSeekerUser = @helpSeekerId
                             );";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@helpSeekerId", helpSeekerId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                        });
                    }
                }
            }
            return results;
        }

        public GeoCoordinate GetVolunteerLocation(int userId)
        {
            GeoCoordinate location = new GeoCoordinate();
            string query = @"SELECT Location.Long, Location.Lat
                             FROM Volunteer
                             WHERE Id = @Id;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@Id", userId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        location.Longitude = reader.GetDouble(0);
                        location.Latitude = reader.GetDouble(1);
                    }
                }
            }
            return location;
        }

        public List<UserModel> Get()
        {
            List<UserModel> results = new List<UserModel>();
            string query = @"SELECT Id, Role, Email, Name, Approved 
                             FROM [User];";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Role = (UserRole)reader.GetInt32(1),
                            Email = reader.GetString(2),
                            Name = reader.GetString(3),
                            Approved = reader.GetBoolean(4)
                        });
                    }
                }
            }
            return results;
        }
        public void Zoom(int zoom, int userId)
        {
            List<UserModel> results = new List<UserModel>();
            string query = @"UPDATE [User]
                             SET Zoom = @zoom
                             WHERE Id = @id;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@zoom", zoom);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();

            }
        }

        public void Password(string password, int userId)
        {
            string query = @"UPDATE [User] 
                             SET Password = @Password
                             WHERE Id = @Id;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
