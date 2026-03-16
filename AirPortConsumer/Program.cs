using System.Text;
using System.Text.Json;
using AirportModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


// Check if any argument is provided
string terminalArg;

// Check if an argument was provided
if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
{
    terminalArg = args[0].Trim().ToLowerInvariant();
}
else
{
    terminalArg = "indenrigs"; // default value
}

// Validate terminalArg
if (terminalArg != "indenrigs" && terminalArg != "udenrigs")
{
    Console.WriteLine("Usage: AirPortConsumer [indenrigs|udenrigs]");
    return;
}

// terminalArg is now guaranteed to be either "indenrigs" or "udenrigs"
Console.WriteLine($"Terminal valgt: {terminalArg}");


var planes = new Dictionary<string, Plane>();

var factory = new ConnectionFactory { HostName = "localhost" };

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

const string exchangeName = "flight_topic_exchange";

await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic);

var queue = await channel.QueueDeclareAsync(
    queue: $"screen.{terminalArg}",
    durable: true,
    exclusive: false,
    autoDelete: false);

var bindingKey = $"plane.{terminalArg}.*";
await channel.QueueBindAsync(queue: queue.QueueName, exchange: exchangeName, routingKey: bindingKey);

Console.WriteLine($"[*] Listening for {terminalArg} updates on '{bindingKey}'...");
Console.WriteLine("[*] Press ENTER to exit.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (_, ea) => {
    
    try {
        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
        var plane = JsonSerializer.Deserialize<Plane>(json);

        if (plane is null || string.IsNullOrWhiteSpace(plane.PlaneNumber))
            return;
        
        planes[plane.PlaneNumber] = plane;
        
        Console.Clear();
        Console.WriteLine("==============================================================");
        Console.WriteLine("                 AIRPORT FLIGHT INFO SCREEN                  ");
        Console.WriteLine("==============================================================");
        Console.WriteLine();

        Console.WriteLine("+------------+----------------------+------------------+------+------------+");
        Console.WriteLine("| Flight No  | Destination          | Departure        | Gate | Status     |");
        Console.WriteLine("+------------+----------------------+------------------+------+------------+");

        foreach (var f in planes.Values.OrderBy(f => f.DepartureTime))
        {
            var flightNo = (f.PlaneNumber ?? "").PadRight(10).Substring(0, 10);
            var destination = (f.Destination ?? "").PadRight(20).Substring(0, 20);
            var departure = f.DepartureTime.ToString("yyyy-MM-dd HH:mm");
            var gate = (f.Gate ?? "").PadRight(4).Substring(0, 4);
            var status = (f.Status ?? "").PadRight(10).Substring(0, 10);
            var terminal = (f.TerminalType ?? "").PadRight(10).Substring(0, 10);

            Console.WriteLine($"| {flightNo} | {destination} | {departure} | {gate} | {status} |  {terminal} |");
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
