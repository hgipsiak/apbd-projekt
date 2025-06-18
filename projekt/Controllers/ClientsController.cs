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
        public async Task<IActionResult> AddNewPerson(PersonClientDto dto)
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
                await _dbService.UpdatePerson(idPerson, dto);
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
                await _dbService.DeletePerson(idPerson);
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
                await _dbService.AddNewCompany(dto);
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
                await _dbService.UpdateCompany(idCompany, dto);
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

        [HttpPost("createPayment/{idClient}/software/{idSoftware}")]
        public async Task<IActionResult> AddPaymentToClient(int idClient, int idSoftware, PaymentDto dto)
        {
            try
            {
                await _dbService.CreatePayment(idClient, idSoftware, dto);
                return Created("newPayment", dto);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (BadRequestException e)
            {
                return BadRequest(e.Message);
            }
            catch (ConflictException e)
            {
                return Conflict(e.Message);
            }
        }
    }
}
