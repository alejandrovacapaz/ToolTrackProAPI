using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ToolTrackPro.Models.Dtos;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories.Imp
{
    public class ToolRepository : BaseRepository, IToolRepository
    {
        public ToolRepository(IConfiguration config) : base(config)
        {
        }
        public async Task<List<ToolAssignmentDto>> GetAllAsync()
        {
            var tools = new List<ToolAssignmentDto>();            

            using var conn = GetConnection();            
            try
            {
                var query = @"
                SELECT 
                    t.Id AS ToolId,
                    t.Name AS ToolName,
                    t.Description,
                    u.Name AS UserName,
                    u.Email AS UserEmail,
                    b.Id AS BorrowId,
                    b.BorrowDate,
                    r.ReturnDate,
                    ta.IsAvailable                    
                FROM Tools t
                INNER JOIN ToolAvailability ta ON t.Id = ta.ToolId
                LEFT JOIN Borrows b ON t.Id = b.ToolId
                AND b.Id = (
                    SELECT TOP 1 Id FROM Borrows 
                    WHERE ToolId = t.Id 
                    ORDER BY BorrowDate DESC
                )
                LEFT JOIN Users u ON b.UserId = u.Id
                LEFT JOIN Returns r ON b.Id = r.BorrowId";
                using var reader = await DataAccessHelper.ExecuteReaderAsync(conn, query);
                while (reader.Read())
                {
                    tools.Add(new ToolAssignmentDto
                    {
                        ToolId = (int)reader["ToolId"],
                        ToolName = reader["ToolName"].ToString() ?? string.Empty,
                        Description = reader["Description"].ToString() ?? string.Empty,
                        UserName = reader["UserName"]?.ToString() ?? string.Empty,
                        UserEmail = reader["UserEmail"]?.ToString() ?? string.Empty,
                        BorrowId = reader["BorrowId"] != DBNull.Value ? (int)reader["BorrowId"] : 0,
                        BorrowDate = reader["BorrowDate"] != DBNull.Value ? (DateTime?)reader["BorrowDate"] : null,
                        ReturnDate = reader["ReturnDate"] != DBNull.Value ? (DateTime?)reader["ReturnDate"] : null,
                        IsAvailable = (bool)reader["IsAvailable"]
                    });
                }                
            }
            catch (Exception ex)
            {                
                throw new Exception("Error retrieving tools", ex);
            }
            finally
            {             
                conn.Close();
            }

            return tools;
        }
        
        public async Task<bool> IsAvailableAsync(int toolId)
        {
            var isSuccess = false;

            using var conn = GetConnection();            
            try
            {
                var query = "SELECT IsAvailable FROM ToolAvailability WHERE ToolId = @ToolId";               
                var parameters = new[]
                {
                    new SqlParameter("@ToolId", toolId),
                };
                var result = await DataAccessHelper.ExecuteScalarAsync(conn, query, parameters);
                isSuccess = result != null && (bool)result;
            }
            catch (Exception ex)
            {                
                throw new Exception("Error checking tool availability", ex);
            }
            finally
            {                
                conn.Close();
            }

            return isSuccess;
        }

        public async Task<bool> ExistsAsync(int toolId)
        {
            var isSuccess = false;

            using var conn = GetConnection();           
            try
            {
                var query = "SELECT COUNT(*) FROM Tools WHERE Id = @ToolId";
                var parameters = new[]
                {
                    new SqlParameter("@ToolId", toolId),
                };
                var result = await DataAccessHelper.ExecuteScalarAsync(conn, query, parameters);
                isSuccess = result != null && (int)result > 0;
            }
            catch (Exception ex)
            {                
                throw new Exception("Error checking tool existence", ex);
            }
            finally
            {                
                conn.Close();
            }

            return isSuccess;
        }

        public async Task<List<OverdueToolDto>> GetOverdueToolsAsync()
        {
            var result = new List<OverdueToolDto>();

            using var conn = GetConnection();
            try
            {
                var query = @"
                SELECT b.ToolId, t.Name, b.UserId, b.BorrowDate, b.EstimatedReturnDate
                FROM Borrows b
                INNER JOIN Tools t ON b.ToolId = t.Id
                LEFT JOIN Returns r ON b.Id = r.BorrowId
                WHERE r.Id IS NULL AND DATEDIFF(DAY, b.EstimatedReturnDate, GETDATE()) > 0";
                
                var reader = await DataAccessHelper.ExecuteReaderAsync(conn, query);
                while (reader.Read())
                {
                    var borrowDate = (DateTime)reader["BorrowDate"];
                    var estimatedReturn = (DateTime)reader["EstimatedReturnDate"];
                    var daysOverdue = (DateTime.Now - estimatedReturn).Days;

                    result.Add(new OverdueToolDto
                    {
                        ToolId = (int)reader["ToolId"],
                        ToolName = reader["Name"].ToString() ?? string.Empty,
                        BorrowedByUserId = (int)reader["UserId"],
                        BorrowedAt = borrowDate,
                        DaysOverdue = daysOverdue
                    });
                }                
            }
            catch (Exception)
            {
                throw new Exception("Failed Getting Overdue Tools");
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public async Task<bool> InsertAsync(Tool tool)
        {
            var isSuccess = false;

            using var conn = GetConnection();
            using var transaction = conn.BeginTransaction();
            try
            {
                var insertQuery = "INSERT INTO Tools (Name, Description) OUTPUT INSERTED.Id VALUES (@Name, @Description)";
                var insertParameters = new[]
                {                    
                    new SqlParameter("@Name", tool.Name),
                    new SqlParameter("@Description", tool.Description),                    
                };

                var toolResult = await DataAccessHelper.ExecuteScalarAsync(conn, insertQuery, insertParameters, transaction);
                if (toolResult == null)
                {
                    throw new Exception("Failed to insert Tool record.");
                }

                var toolId = (int)toolResult;

                var insertAvailabilityQuery = "INSERT INTO ToolAvailability (ToolId, IsAvailable) VALUES (@ToolId, @IsAvailable)";
                var insertAvailabilityParameters = new[]
                {
                    new SqlParameter("@ToolId", toolId),
                    new SqlParameter("@IsAvailable", 1),
                };

                var availabilityResult = await DataAccessHelper.ExecuteNonQueryAsync(conn, insertAvailabilityQuery, insertAvailabilityParameters, transaction);
                if (availabilityResult <= 0)
                {
                    throw new Exception("Failed to insert Tool Availability record.");
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error inserting Tool record", ex);
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
