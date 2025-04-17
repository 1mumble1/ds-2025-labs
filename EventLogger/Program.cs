using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

internal class Program
{
    private const string QueueName = "event.log.queue";
    private const string ExchangeName = "logs";
    private static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: ExchangeName,
            type: ExchangeType.Fanout);

        // declare a server-named queue
        QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
        await channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: string.Empty);

        Console.WriteLine(" [*] Waiting for logs.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] {message}");
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(QueueName, autoAck: true, consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}