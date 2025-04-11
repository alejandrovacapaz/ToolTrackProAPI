using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolTrackPro.Services;

namespace ToolTrackProAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet("overdue-tool-notification")]
        [Authorize]
        public async Task<IActionResult> OverdueToolNotification()
        {
            try
            {
                await this.notificationService.SendOverdueEmailsAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }                       
        }
    }
}
