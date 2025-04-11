using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories.Imp
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration config) : base(config)
        {
        }

        public async Task<User> LoginAsync(User user)
        {
            var resultUser = new User();            
            using var conn = GetConnection();

            try
            {
                var query = @"SELECT Id, Name, PasswordHash FROM Users WHERE Email = @Email";
                var parameters = new[]
                {
                    new SqlParameter("@Email", user.Email)
                };

                using var reader = await DataAccessHelper.ExecuteReaderAsync(conn, query, parameters);
                if (!reader.Read()) return null;

                var hash = (byte[])reader["PasswordHash"];
                if (!VerifyPassword(user.PasswordHash, hash)) return null;

                resultUser = new User
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString() ?? string.Empty,
                    Email = user.Email
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error trying to log in", ex);              
            }
            finally
            {
                conn.Close();
            }

            return resultUser;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            var isSuccess = false;
            var hash = HashPassword(user.PasswordHash);
            using var conn = GetConnection();

            if (await EmailAlreadyExistsAsync(user.Email))
            {
                throw new Exception("Email already exists");
            }

            using var transaction = conn.BeginTransaction();
            try
            {
                var query = @"INSERT INTO Users (Name, Email, PasswordHash)
                            VALUES (@Name, @Email, @PasswordHash)";
                var parameters = new[]
                {
                    new SqlParameter("@Name", user.Name),
                    new SqlParameter("@Email", user.Email),
                    new SqlParameter("@PasswordHash", hash),
                };

                isSuccess = await DataAccessHelper.ExecuteNonQueryAsync(conn, query, parameters, transaction) > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error Registering new User", ex);
            }
            finally
            {
                if (isSuccess)
                {
                    transaction.Commit();
                }
                conn.Close();
            }

            return isSuccess;          
        }

        public async Task<string> GetEmailAsync(int userId)
        {
            var email = string.Empty;
            using var conn = GetConnection();

            try
            {
                var query = "SELECT Email FROM Users WHERE Id = @UserId";
                var parameters = new[]
                {
                    new SqlParameter("@UserId", userId),
                };

                var result = await DataAccessHelper.ExecuteScalarAsync(conn, query, parameters);
                email = result?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Getting User Email", ex);
            }
            finally
            {
                conn.Close();
            }

            return email;
        }

        private async Task<bool> EmailAlreadyExistsAsync(string email) 
        {   
            var isSuccess = false;
            using var conn = GetConnection();

            try
            {
                var query = "SELECT Id FROM Users WHERE Email = @Email";
                var parameters = new[]
                {
                    new SqlParameter("@Email", email),
                };

                var result = await DataAccessHelper.ExecuteScalarAsync(conn, query, parameters);
                isSuccess = result != null && (int)result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Getting User Email", ex);
            }
            finally
            {
                conn.Close();
            }

            return isSuccess;
        }

        private byte[] HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, byte[] hash)
        {
            var inputHash = HashPassword(password);
            return inputHash.SequenceEqual(hash);
        }
    }
}
