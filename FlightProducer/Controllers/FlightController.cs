using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IntegrationProject.Models;
using IntegrationProject.Service;

namespace IntegrationProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightController : ControllerBase {
    private static List<Flight> flights = new List<Flight>();
    private readonly IMessageProducer _messageProducer;

    // Her bruger vi Dependency Injection (DI). 
    // ASP.NET Core leverer automatisk en instans af IMessageProducer til os via constructoren.
    public FlightController(IMessageProducer messageProducer)
    {
        _messageProducer = messageProducer;
    }

    [HttpPost]
    public async Task<IActionResult> CreateFlight([FromBody] Flight newFlight) {
        flights.Add(newFlight);

        await _messageProducer.SendFlightUpdate(newFlight);
        return Ok(flights);
    }

    [HttpGet]
    public async Task<IActionResult> GetFlights() {
        return Ok(flights);
    }

    [HttpPut("{flightNumber}")]
    public async Task<IActionResult> UpdateFlight(string flightNumber, [FromBody] Flight updatedFlight) {
        var flight = flights.FirstOrDefault(f => f.FlightNumber == flightNumber);
        if (flight == null) {
            return NotFound();
        }

        flight.Destination = updatedFlight.Destination;
        flight.DepartureTime = updatedFlight.DepartureTime;
        flight.Gate = updatedFlight.Gate;
        flight.Status = updatedFlight.Status;

        await _messageProducer.SendFlightUpdate(updatedFlight);
        return Ok(flight);
    }

    [HttpDelete("{flightNumber}")]
    public async Task<IActionResult> DeleteFlight(string flightNumber) {
        var flight = flights.FirstOrDefault(f => f.FlightNumber == flightNumber);

        if (flight == null) {
            return NotFound();
        }

        flights.Remove(flight);

        flight.Status = "Cancelled";
        await _messageProducer.SendFlightUpdate(flight);
        return NoContent();
    }
}