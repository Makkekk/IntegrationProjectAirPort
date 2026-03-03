namespace IntegrationProject.Models;

public class Flight
{
    public string FlightNumber { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public string Gate { get; set; }
    public string Status { get; set; } //OnTime, Boarding,Delayed, Cancelled
}