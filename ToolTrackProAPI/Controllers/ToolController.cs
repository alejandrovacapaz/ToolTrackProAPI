using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolTrackPro.Models.Models;
using ToolTrackPro.Services;

namespace ToolTrackProAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToolController : ControllerBase
    {
        private readonly IToolService toolService;       

        public ToolController(IToolService toolService)
        {
            this.toolService = toolService;            
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var result = await this.toolService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
            
        }

        [HttpPost("borrow")]
        [Authorize]
        public async Task<IActionResult> BorrowToolAsync([FromBody] Borrow borrow)
        {
            try
            {
                var result = await toolService.BorrowAsync(borrow);
                return result ? Ok(result) : BadRequest("Tool is not available.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }            
        }

        [HttpPost("return")]
        [Authorize]
        public async Task<IActionResult> ReturnToolAsync([FromBody] Return returned)
        {
            try
            {
                var result = await toolService.ReturnAsync(returned);
                return result ? Ok(result) : BadRequest("Return failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
            
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddAsync([FromBody] Tool tool)
        {
            try
            {
                var result = await this.toolService.AddAsync(tool);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
            
        }
    }
}
