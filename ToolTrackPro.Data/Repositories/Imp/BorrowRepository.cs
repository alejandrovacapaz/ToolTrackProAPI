using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories.Imp
{
    public class BorrowRepository : BaseRepository, IBorrowRepository
    {
        public BorrowRepository(IConfiguration config) : base(config) 
        {
        }

        public async Task<bool> InsertAsync(Borrow borrow)
        {            
            var isSuccess = false;

            using var conn = GetConnection();
            using var transaction = conn.BeginTransaction();
            try
            {
                var insertQuery = "INSERT INTO Borrows (ToolId, UserId, BorrowDate, EstimatedReturnDate) " +
                              "VALUES (@ToolId, @UserId, @BorrowDate, @EstimatedReturnDate)";                
                var insertParameters = new[]
                {
                    new SqlParameter("@ToolId", borrow.ToolId),
                    new SqlParameter("@UserId", borrow.UserId),
                    new SqlParameter("@BorrowDate", borrow.BorrowDate),
                    new SqlParameter("@EstimatedReturnDate", borrow.EstimatedReturnDate)
                };                

                var borrowResult = await DataAccessHelper.ExecuteNonQueryAsync(conn, insertQuery, insertParameters, transaction);
                if (borrowResult <= 0)
                {
                    throw new Exception("Failed to insert Borrow record.");
                }

                // Update ToolAvailability to mark tool as unavailable
                var updateQuery = "UPDATE ToolAvailability SET IsAvailable = 0 WHERE ToolId = @ToolId";                
                var updateParameters = new[]
                {
                    new SqlParameter("@ToolId", borrow.ToolId),                  
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
                throw new Exception("Error inserting borrow record", ex);
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
