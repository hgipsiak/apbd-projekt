using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projekt.DTOs;
using projekt.Exceptions;
using projekt.Services;

namespace projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Regular")]
    public class ProfitsController : ControllerBase
    {
        private readonly IProfitsService _profitsService;

        public ProfitsController(IProfitsService profitsService)
        {
            _profitsService = profitsService;
        }

        [HttpGet("{currencyCode}")]
        public async Task<IActionResult> GetProfit(string currencyCode, [FromQuery] int? softwareId = null)
        {
            try
            {
                GetProfitDto res = await _profitsService.CalculateProfit(currencyCode, softwareId);
                return Ok(res);
            }
            catch (BadRequestException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
