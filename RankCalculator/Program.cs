using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Services;
using System.Text;

public class Program
{
    private static IRedisService _redis = new RedisService("localhost:6379");
    private const string QueueName = "valuator.processing.rank";

    private static async Task Main(string[] args)
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = "localhost",
        };
        await using IConnection connection = await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        await DeclareTopologyAsync(channel);
        string consumerTag = await RunConsumer(channel);

        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();

        await channel.BasicCancelAsync(consumerTag);

        Console.WriteLine("done");
    }

    private static async Task<string> RunConsumer(IChannel channel)
    {
        AsyncEventingBasicConsumer consumer = new(channel);
        consumer.ReceivedAsync += (_, eventArgs) => ConsumeAsync(channel, eventArgs);
        return await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );
    }

    private static async Task ConsumeAsync(IChannel channel, BasicDeliverEventArgs eventArgs)
    {
        Console.WriteLine("Consuming");
        string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        string text = SearchTextById(message) ?? throw new Exception("Error of getting string from database");

        string rank = CalculateRank(text);
        _redis.SetString($"RANK-{message}", rank);

        await SendLogMessage(channel, message, rank);

        Console.WriteLine($"Consuming: {message} from subject {eventArgs.Exchange}");
        await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
    }

    private static async Task SendLogMessage(IChannel channel, string id, string value)
    {
        var logMessage = $"RANK-{id}: {value}";
        var body = Encoding.UTF8.GetBytes(logMessage);
        await channel.BasicPublishAsync(exchange: "logs", routingKey: string.Empty, body: body);
        Console.WriteLine($" [x] Sent {logMessage}");
    }

    private static string? SearchTextById(string message) => _redis.GetString($"TEXT-{message}");

    private static bool IsLatinOrCyrillic(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||
               (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я') ||
               (c == 'Ё') ||
               (c == 'ё');
    }

    private static string CalculateRank(string text)
    {
        double length = text.Length;
        double counterNonalphabetSymbols = text.Count(c =>
            !IsLatinOrCyrillic(c));

        return (counterNonalphabetSymbols / length).ToString();
    }


    /// <summary>
    ///  Определяет топологию: queue -> consumer.
    /// </summary>
    private static async Task DeclareTopologyAsync(IChannel channel)
    {
        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
        await channel.ExchangeDeclareAsync(
            exchange: "logs",
            type: ExchangeType.Fanout
        );
    }
}