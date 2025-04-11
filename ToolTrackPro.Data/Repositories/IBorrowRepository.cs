using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories
{
    public interface IBorrowRepository
    {
        Task<bool> InsertAsync(Borrow borrow);
    }
}
