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
    [Authorize(Roles = "Admin")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [HttpPost("addPerson")]
        public async Task<IActionResult> AddNewPerson(PersonClientDto dto)
        {
            try
            {
                await _clientsService.AddNewPerson(dto);
                return Created("newPerson", dto);
            }
            catch (ConflictException e)
            {
                return Conflict(e.Message);
            }
            catch (BadRequestException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("updatePerson/{idPerson}")]
        public async Task<IActionResult> UpdatePerson(int idPerson, PersonClientDto dto)
        {
            try
            {
                await _clientsService.UpdatePerson(idPerson, dto);
                return Ok("Person updated");
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (BadRequestException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("deletePerson/{idPerson}")]
        public async Task<IActionResult> DeletePerson(int idPerson)
        {
            try
            {
                await _clientsService.DeletePerson(idPerson);
                return Ok("Person deleted");
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("addCompany")]
        public async Task<IActionResult> AddNewCompany(CompanyClientDto dto)
        {
            try
            {
                await _clientsService.AddNewCompany(dto);
                return Created("newCompany", dto);
            }
            catch (ConflictException e)
            {
                return Conflict(e.Message);
            }
            catch (BadRequestException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("updateCompany/{idCompany}")]
        public async Task<IActionResult> UpdateCompany(int idCompany, CompanyClientDto dto)
        {
            try
            {
                await _clientsService.UpdateCompany(idCompany, dto);
                return Ok("Company updated");
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (BadRequestException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
