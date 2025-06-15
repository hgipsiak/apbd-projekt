using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projekt.DTOs;
using projekt.Exceptions;
using projekt.Services;

namespace projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public ClientsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("addPerson")]
        public async Task<IActionResult> AddNewPerson(AddPersonClientDto dto)
        {
            try
            {
                await _dbService.AddNewPerson(dto);
                return Created("newPerson", dto);
            }
            catch (ConflictException e)
            {
                return Conflict(e.Message);
            }
        }
    }
}
