namespace AirportModels;

public class Plane {
    public string PlaneNumber { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public string Gate { get; set; } = string.Empty;
    public string Status { get; set; } = "On Time"; //OnTime, Boarding,Delayed, Cancelled
    public string TerminalType { get; init; } = "Indenrigs"; //Indrigs eller udenrigss

}