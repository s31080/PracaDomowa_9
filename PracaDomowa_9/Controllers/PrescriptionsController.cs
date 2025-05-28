using Microsoft.AspNetCore.Mvc;
using PracaDomowa_9.DbService;
using PracaDomowa_9.DTOs;
using PracaDomowa_9.Exceptions;

namespace PracaDomowa_9.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionsController(IDbService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionCreateDto dto)
    {
        try
        {
            await service.AddPrescriptionAsync(dto);
            return Created("", null);
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

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    [HttpGet("patient/{id}")]
    public async Task<IActionResult> GetPatientDetails([FromRoute] int id)
    {
        try
        {
            var result = await service.GetPatientDetailsAsync(id);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
