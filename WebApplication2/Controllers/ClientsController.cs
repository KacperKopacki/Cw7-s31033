using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;
// kontroler od operacji na klientach
[ApiController]
[Route("api/[controller]")]
public class ClientsController(IDbService service) : ControllerBase
{
    [HttpGet("{id}/trips")] //sciezka
    public async Task<IActionResult> GetClientTrips(int id) // zwraca wycieczki klienta
    {
        var trips = await service.GetClientTrips(id);
        if (trips == null || !trips.Any())
        {
            return NotFound(); // 404 jesli brak wycieczek
        }
        return Ok(trips); // 200 jesli sa wycieczki
    }

    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] ClientPostDTO dto) // dodanie nowego klienta
    {
        if (!ModelState.IsValid) // sprawdzenie poprawnosci danych 
        {
            return BadRequest(ModelState);
        }
        var client = await service.AddClient(dto);
        return StatusCode(201, new {id = client}); // 201 i id klienta
    }
}