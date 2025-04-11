using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories.Imp
{
    public class ReturnRepository : BaseRepository, IReturnRepository
    {
        public ReturnRepository(IConfiguration config) : base(config)
        {
        }

        public async Task<bool> InsertAsync(Return returned)
        {
            var isSuccess = false;

            using var conn = GetConnection();
            using var transaction = conn.BeginTransaction();
            try
            {
                var insertQuery = "INSERT INTO Returns (BorrowId, ReturnDate) VALUES (@BorrowId, @ReturnDate)";
                var insertParameters = new[]
                {
                    new SqlParameter("@BorrowId", returned.BorrowId),
                    new SqlParameter("@ReturnDate", returned.ReturnDate),                    
                };

                var returnResult = await DataAccessHelper.ExecuteNonQueryAsync(conn, insertQuery, insertParameters, transaction);
                if (returnResult <= 0)
                {
                    throw new Exception("Failed to insert Return record.");
                }

                // Update ToolAvailability to mark tool as unavailable
                var query = "SELECT ToolId FROM Borrows WHERE Id = @BorrowId";
                var queryParameters = new[]
                {
                    new SqlParameter("@BorrowId", returned.BorrowId)                    
                };

                var toolResult = await DataAccessHelper.ExecuteScalarAsync(conn, query, queryParameters, transaction);
                var toolId = toolResult != null ? (int)toolResult : 0;
                if (toolId <= 0)
                {
                    throw new Exception("Failed to retrieve ToolId from Borrows.");
                }

                var updateQuery = "UPDATE ToolAvailability SET IsAvailable = 1 WHERE ToolId = @ToolId";
                var updateParameters = new[]
                {
                    new SqlParameter("@ToolId", toolId),
                };

                var availabilityUpdateResult = await DataAccessHelper.ExecuteNonQueryAsync(conn, updateQuery, updateParameters, transaction);
                if (availabilityUpdateResult <= 0)
                {
                    throw new Exception("Failed to update ToolAvailability.");
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error inserting return record", ex);
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
