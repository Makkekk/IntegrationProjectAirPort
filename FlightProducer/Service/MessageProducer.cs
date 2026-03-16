using AirportModels;
using RabbitMQ.Client;

namespace IntegrationProject.Service;

public class MessageProducer {
   private const string ExchangeName = "flight_topic_exchange";
    
    public async Task SendFlightUpdate(Plane plane) 
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        
        
        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic);
        
        var routingkey = $"flight.{plane.Status.ToLower()}";
        
        var serializedPlane = System.Text.Json.JsonSerializer.Serialize(plane);
        var body = System.Text.Encoding.UTF8.GetBytes(serializedPlane);
        
        await channel.BasicPublishAsync(exchange: ExchangeName, routingKey: routingkey, body: body);
        
        Console.WriteLine($"[PRODUCER] Besked sendt for {plane.PlaneNumber} til {routingkey}" );
    }
}