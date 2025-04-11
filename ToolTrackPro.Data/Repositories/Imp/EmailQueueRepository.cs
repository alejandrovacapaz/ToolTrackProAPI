using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories.Imp
{
    public class EmailQueueRepository: BaseRepository, IEmailQueueRepository
    {
        public EmailQueueRepository(IConfiguration config) : base(config)
        {
        }

        public async Task<bool> InsertAsync(EmailQueue emailQueue)
        {
            var isSuccess = false;
            using var conn = GetConnection();

            using var transaction = conn.BeginTransaction();
            try
            {
                var insertQuery = "INSERT INTO EmailQueue (ToEmail, Subject, Body) VALUES (@ToEmail, @Subject, @Body)";
                var parameters = new[]
                {
                    new SqlParameter("@ToEmail", emailQueue.ToEmail),
                    new SqlParameter("@Subject", emailQueue.MailSubject),
                    new SqlParameter("@Body", emailQueue.Body)
                };
                var result = await DataAccessHelper.ExecuteNonQueryAsync(conn, insertQuery, parameters, transaction);
                if (result <= 0)
                {
                    throw new Exception("Failed to insert EmailQueue record.");
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error inserting email queue record", ex);
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

        
    }
}
