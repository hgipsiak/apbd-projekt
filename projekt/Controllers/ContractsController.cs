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
    public class ContractsController : ControllerBase
    {
        private readonly IContractsService _contractsService;

        public ContractsController(IContractsService contractsService)
        {
            _contractsService = contractsService;
        }
        
        [HttpPost("createContract/{idClient}/software/{idSoftware}")]
        public async Task<IActionResult> AddContractToClient(int idClient, int idSoftware, ContractDto dto)
        {
            try
            {
                await _contractsService.CreateContract(idClient, idSoftware, dto);
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

        [HttpDelete("deleteContract/{idContract}")]
        public async Task<IActionResult> DeleteContract(int idContract)
        {
            try
            {
                await _contractsService.DeleteContract(idContract);
                return Ok("Contract deleted");
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("payContract/{idContract}")]
        public async Task<IActionResult> PayContract(int idContract)
        {
            try
            {
                await _contractsService.PayContract(idContract);
                return Created("newPayment", "Payment created");
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
