using System.Text;
using System.Text.Json;
using AirportModels;
using RabbitMQ.Client;

namespace IntegrationProject.Service;

public class MessageProducer {
    private const string ExchangeName = "flight_topic_exchange";

    public async Task SendFlightUpdate(Plane plane) {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();


        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic);

        var terminal = NormalizeTerminal(plane.TerminalType); // indenrigs / udenrigs
        var status = NormalizeStatus(plane.Status); //Delayed / on time / etc
        var routingkey = $"plane.{terminal}.{status}";

        var payload = JsonSerializer.Serialize(plane);
        var body = Encoding.UTF8.GetBytes(payload);

        await channel.BasicPublishAsync(exchange: ExchangeName, routingKey: routingkey, body: body);

        Console.WriteLine($"[PRODUCER] Besked sendt for {plane.PlaneNumber} til {routingkey}");
    }


    private static String NormalizeTerminal(String? terminalType) {
        if (string.IsNullOrWhiteSpace(terminalType))
            return "ukendt"; // default

        var value = terminalType.Trim().ToLowerInvariant();

        if (value == "indenrigs")
            return "indenrigs";

        if (value == "udenrigs")
            return "udenrigs";

        return "ukendt";
    }

    private static string NormalizeStatus(string? status) {
        if (string.IsNullOrWhiteSpace(status))
            return "unknown";

        // "On Time" -> "ontime"
        var value = status.Trim().ToLowerInvariant().Replace(" ", "");

        if (value == "ontime")
            return "ontime";

        if (value == "boarding")
            return "boarding";

        if (value == "delayed")
            return "delayed";

        if (value == "cancelled")
            return "cancelled";

        return "unknown";
    }
}