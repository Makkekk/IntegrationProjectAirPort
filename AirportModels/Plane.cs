namespace AirportModels;

public class Plane
{
    public string PlaneNumber { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public string Gate { get; set; }
    public string Status { get; set; } //OnTime, Boarding,Delayed, Cancelled
    public string TerminalType { get; set; } //Indrigs eller udenrigss
    
}