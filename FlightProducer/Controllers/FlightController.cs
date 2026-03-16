using System.Diagnostics;
using AirportModels;
using Microsoft.AspNetCore.Mvc;
using IntegrationProject.Service;

namespace IntegrationProject.Controllers;

[ApiController]
[Route("api/[controller]")]

public class FlightController : ControllerBase {
    
    private static List<Plane> Planes = new List<Plane>();
    private readonly MessageProducer messageProducer = new MessageProducer();

    [HttpPost]
    public async Task<IActionResult> CreateFlight([FromBody] Plane newPlane) {
        if (!IsValidTerinal(newPlane.TerminalType))
            return BadRequest("Terminal type skal være 'indenrigs' eller 'udenrigs'. ");
        
        Planes.Add(newPlane);
        await messageProducer.SendFlightUpdate(newPlane);
        return Ok(Planes);
    }

    [HttpGet]
    public async Task<IActionResult> GetFlights() {
        return Ok(Planes);
    }

    [HttpPut("{flightNumber}")]
    public async Task<IActionResult> UpdateFLight(string flightNumber, [FromBody] Plane updatedPlane) {
        var flight = Planes.FirstOrDefault(f => f.PlaneNumber == flightNumber);
        if (flight == null) {
            return NotFound();
        }

        flight.Destination = updatedPlane.Destination;
        flight.DepartureTime = updatedPlane.DepartureTime;
        flight.Gate = updatedPlane.Gate;
        flight.Status = updatedPlane.Status;

        await messageProducer.SendFlightUpdate(updatedPlane);
        return Ok(flight);
    }

    [HttpDelete("{flightNumber}")]
    public async Task<IActionResult> DeleteFlight(string flightNumber) {
        var plane = Planes.FirstOrDefault(f => f.PlaneNumber == flightNumber);

        if (plane == null) {
            return NotFound();
        }

        Planes.Remove(plane);

        plane.Status = "Cancelled";
        await messageProducer.SendFlightUpdate(plane);
        return NoContent();
    }

    private static bool IsValidTerinal(string? terminalType) {
        var terminal = (terminalType ?? "").Trim().ToLowerInvariant();
        return terminal == "indenrigs" || terminal == "udenrigs";
    }
}