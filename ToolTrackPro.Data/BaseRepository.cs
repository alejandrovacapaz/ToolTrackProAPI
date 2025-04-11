using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ToolTrackPro.Data
{
    public abstract class BaseRepository
    {
        protected readonly string connectionString;
        
        protected BaseRepository(IConfiguration config)
        {            
            connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        protected SqlConnection GetConnection()
        {
            try
            {
                var conn = new SqlConnection(this.connectionString);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                throw new Exception("Error connection to the database", ex);
            }
            
        }
    }
}
