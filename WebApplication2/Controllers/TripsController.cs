using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController] // kontroler od operacji na wycieczkach
[Route("api/[controller]")]
public class TripsController(IDbService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTravels() // zwraca wycieczki z krajmi
    {
        return Ok(await service.GetAllTrips()); // 200 z wycieczkami
    }
}
