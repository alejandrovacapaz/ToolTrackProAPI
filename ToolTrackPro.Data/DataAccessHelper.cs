using Microsoft.Data.SqlClient;
using System.Data;

namespace ToolTrackPro.Data
{
    public static class DataAccessHelper
    {        
        public static async Task<int> ExecuteNonQueryAsync(SqlConnection conn, string query, SqlParameter[] parameters, SqlTransaction trans)
        {
            try
            {
                using var cmd = new SqlCommand(query, conn, trans);
                if (parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Executing ExecuteNonQueryAsync", ex);
            }           
        }

        public static async Task<object?> ExecuteScalarAsync(SqlConnection conn, string query, SqlParameter[] parameters, SqlTransaction? trans = null)
        {
            try
            {
                using var cmd = trans == null
                    ? new SqlCommand(query, conn)
                    : new SqlCommand(query, conn, trans);
                if (parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return await cmd.ExecuteScalarAsync() ?? null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Executing ExecuteScalarAsync", ex);
            }            
        }

        public static async Task<SqlDataReader> ExecuteReaderAsync(SqlConnection conn, string query, params SqlParameter[] parameters)
        {
            try
            {
                var cmd = new SqlCommand(query, conn);
                if (parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Executing ExecuteReaderAsync", ex);
            }
            
        }
    }
}
