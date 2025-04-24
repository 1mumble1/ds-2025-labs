using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IRedisService _redisService;

    public SummaryModel(ILogger<SummaryModel> logger, IRedisService redisService)
    {
        _logger = logger;
        _redisService = redisService;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }
    public bool IsCalculated { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        string region = _redisService.GetString("main", id) ?? throw new Exception("Error of getting string from database");

        string rankString = _redisService.GetString(region, "RANK-" + id) ?? "";

        IsCalculated = !string.IsNullOrEmpty(rankString);

        if (IsCalculated)
        {
            if (double.TryParse(rankString, out double rank))
            {
                Rank = rank;
            }
            else
            {
                Rank = 0;
                IsCalculated = false;
                Console.WriteLine("Ошибка преобразования!");
            }
        }

        string similarityString = _redisService.GetString(region, "SIMILARITY-" + id) ?? "";
        if (double.TryParse(similarityString, out double similarity))
        {
            Similarity = similarity;
        }
        else
        {
            Similarity = 0;
            Console.WriteLine("Ошибка преобразования!");
        }
    }
}
