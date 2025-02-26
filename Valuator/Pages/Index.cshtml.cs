using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Services;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRedisService _redisService;

    public IndexModel(ILogger<IndexModel> logger, IRedisService redisService)
    {
        _logger = logger;
        _redisService = redisService;
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);
        if (string.IsNullOrEmpty(text)) return Redirect("");

        string id = Guid.NewGuid().ToString();

        string similarityKey = "SIMILARITY-" + id;
        // TODO: (pa1) посчитать similarity и сохранить в БД (Redis) по ключу similarityKey
        _redisService.SetString(similarityKey, CalculateSimilarity(text));

        string textKey = "TEXT-" + id;
        // TODO: (pa1) сохранить в БД (Redis) text по ключу textKey
        _redisService.SetString(textKey, text);

        string rankKey = "RANK-" + id;
        // TODO: (pa1) посчитать rank и сохранить в БД (Redis) по ключу rankKey
        _redisService.SetString(rankKey, CalculateRank(text));

        return Redirect($"summary?id={id}");
    }

    private bool IsLatinOrCyrillic(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||
               (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я') ||
               (c == 'Ё') ||
               (c == 'ё');
    }

    private string CalculateRank(string text)
    {
        double length = text.Length;
        double counterNonalphabetSymbols = text.Count(c => 
            !IsLatinOrCyrillic(c));

        return (counterNonalphabetSymbols / length).ToString();
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
