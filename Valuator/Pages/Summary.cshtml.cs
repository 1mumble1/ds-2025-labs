using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Valuator.Services;

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

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        // TODO: (pa1) проинициализировать свойства Rank и Similarity значениями из БД (Redis)
        string rankString = _redisService.GetString("RANK-" + id) ?? "";
        if (double.TryParse(rankString, out double rank))
        {
            Rank = rank;
        }
        else
        {
            Rank = 0;
            Console.WriteLine("Ошибка преобразования!");
        }

        string similarityString = _redisService.GetString("SIMILARITY-" + id) ?? "";
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
