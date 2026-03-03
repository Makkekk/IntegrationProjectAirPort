using System.Text;
using System.Text.Json;
using IntegrationProject.Models;
using RabbitMQ.Client;

namespace IntegrationProject.Service;

public class MessageProducer {
    private const string ExchangeName = "flight_fanout_exchange";
    
    public async Task SendFlightUpdate(Flight flight) 
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        
        const string exchangeName = "flight_fanout_exchange";
        
        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout);

        var message = JsonSerializer.Serialize(flight);
        var body = Encoding.UTF8.GetBytes(message);
        
        await channel.BasicPublishAsync(exchange: exchangeName, routingKey: "", body: body);
    
        Console.WriteLine($"[PRODUCER] Besked sendt for {flight.FlightNumber}");
    }
}