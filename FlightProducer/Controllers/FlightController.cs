using System.Diagnostics;
using AirportModels;
using Microsoft.AspNetCore.Mvc;
using IntegrationProject.Service;

namespace IntegrationProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightController : ControllerBase {
    private static List<Plane> flights = new List<Plane>();
    private readonly MessageProducer messageProducer = new MessageProducer();

    [HttpPost]
    public async Task<IActionResult> CreateFlight([FromBody] Plane newPlane) {
        flights.Add(newPlane);

        await messageProducer.SendFlightUpdate(newPlane);
        return Ok(flights);
    }

    [HttpGet]
    public async Task<IActionResult> getFlights() {
        return Ok(flights);
    }

    [HttpPut("{flightNumber}")]
    public async Task<IActionResult> UpdateFLight(string flightNumber, [FromBody] Plane updatedPlane) {
        var flight = flights.FirstOrDefault(f => f.PlaneNumber == flightNumber);
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
        var flight = flights.FirstOrDefault(f => f.PlaneNumber == flightNumber);

        if (flight == null) {
            return NotFound();
        }

        flights.Remove(flight);

        flight.Status = "Cancelled";
        await messageProducer.SendFlightUpdate(flight);
        return NoContent();
    }
}