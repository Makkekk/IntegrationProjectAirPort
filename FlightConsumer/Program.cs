using System.Text;
using System.Text.Json;
using IntegrationProject.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var flights = new Dictionary<string, Flight>();

var factory = new ConnectionFactory { HostName = "localhost" };

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

const string exchangeName = "flight_fanout_exchange";

await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout);

var queue = await channel.QueueDeclareAsync();
await channel.QueueBindAsync(queue: queue.QueueName, exchange: exchangeName, routingKey: "");

Console.WriteLine("[*] Listening for flight updates...");
Console.WriteLine("[*] Press ENTER to exit.");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (_, ea) =>
{
    try
    {
        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
        var flight = JsonSerializer.Deserialize<Flight>(json);

        if (flight is null || string.IsNullOrWhiteSpace(flight.FlightNumber))
            return;
        
        flights[flight.FlightNumber] = flight;

        
        Console.Clear();
        Console.WriteLine("==============================================================");
        Console.WriteLine("                 AIRPORT FLIGHT INFO SCREEN                  ");
        Console.WriteLine("==============================================================");
        Console.WriteLine();

        Console.WriteLine("+------------+----------------------+------------------+------+------------+");
        Console.WriteLine("| Flight No  | Destination          | Departure        | Gate | Status     |");
        Console.WriteLine("+------------+----------------------+------------------+------+------------+");

        foreach (var f in flights.Values.OrderBy(f => f.DepartureTime))
        {
            var flightNo = (f.FlightNumber ?? "").PadRight(10).Substring(0, 10);
            var destination = (f.Destination ?? "").PadRight(20).Substring(0, 20);
            var departure = f.DepartureTime.ToString("yyyy-MM-dd HH:mm");
            var gate = (f.Gate ?? "").PadRight(4).Substring(0, 4);
            var status = (f.Status ?? "").PadRight(10).Substring(0, 10);

            Console.WriteLine($"| {flightNo} | {destination} | {departure} | {gate} | {status} |");
        }

        Console.WriteLine("+------------+----------------------+------------------+------+------------+");
        Console.WriteLine($"Updated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
    }

    await Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: queue.QueueName, autoAck: true, consumer: consumer);

Console.ReadLine();
