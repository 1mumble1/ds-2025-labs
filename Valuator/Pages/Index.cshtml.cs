using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RabbitMQ.Client;
using Services;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRedisService _redisService;
    private const string ExchangeName = "valuator.processing.rank";
    private const string LogExchangeName = "logs";
    private const string QueueName = "valuator.processing.rank";

    public IndexModel(ILogger<IndexModel> logger, IRedisService redisService)
    {
        _logger = logger;
        _redisService = redisService;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync(string text)
    {
        _logger.LogDebug(text);
        if (string.IsNullOrEmpty(text)) return Redirect("/");

        string id = Guid.NewGuid().ToString();

        string similarityKey = "SIMILARITY-" + id;
        // TODO: (pa1) посчитать similarity и сохранить в БД (Redis) по ключу similarityKey
        string similarity = CalculateSimilarity(text);
        _redisService.SetString(similarityKey, similarity);

        string textKey = "TEXT-" + id;
        // TODO: (pa1) сохранить в БД (Redis) text по ключу textKey
        _redisService.SetString(textKey, text);

        //string rankKey = "RANK-" + id;
        //// TODO: (pa1) посчитать rank и сохранить в БД (Redis) по ключу rankKey
        //_redisService.SetString(rankKey, CalculateRank(text));
        await SendMessageToBrokerAsync(id, similarity);

        return Redirect($"summary?id={id}");
    }

    private async Task SendMessageToBrokerAsync(string id, string similarity)
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        await using IConnection connection = await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        await DeclareTopologyAsync(channel);

        var message = id;
        byte[] messageData = Encoding.UTF8.GetBytes(message);
        await SendLogMessage(channel, id, similarity);

        Console.WriteLine($"Sending message: {message}");
        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: "",
            mandatory: false,
            body: messageData
        );
    }

    private static async Task SendLogMessage(IChannel channel, string id, string similarity)
    {
        var logMessage = $"SIMILARITY-{id}: {similarity}";
        var body = Encoding.UTF8.GetBytes(logMessage);
        await channel.BasicPublishAsync(exchange: LogExchangeName, routingKey: string.Empty, body: body);
        Console.WriteLine($" [x] Sent {logMessage}");
    }

    private async Task DeclareTopologyAsync(IChannel channel)
    {
        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Direct
        );
        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
        await channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: ""
        );
        await channel.ExchangeDeclareAsync(
            exchange: LogExchangeName, 
            type: ExchangeType.Fanout
        );
    }

    private string CalculateSimilarity(string text)
    {
        List<string> keys = _redisService.GetAllKeys();
        foreach (string key in keys)
        {
            if (key.StartsWith("TEXT-") && _redisService.GetString(key) == text)
            {
                return "1";
            }
        }
        return "0";
    }
}
