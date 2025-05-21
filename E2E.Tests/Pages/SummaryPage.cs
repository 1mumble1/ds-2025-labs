using System.Globalization;
using OpenQA.Selenium;

namespace E2ETests.Pages;

public class SummaryPage
{
    IWebDriver _webDriver;
    private static readonly By
        _rankTextXPath = By.XPath("//p[@id='rank']"),
        _similarityTextXPath = By.XPath("//p[@id='similarity']");

    public SummaryPage(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public IWebElement GetRankText()
    {
        return _webDriver.FindElement(_rankTextXPath);
    }
    public IWebElement GetSimilarityText()
    {
        return _webDriver.FindElement(_similarityTextXPath);
    }

    public bool IsRankEqualTo(string rank)
    {
        string expectedText = $"Оценка содержания: {rank}";

        //int attemps = 5;
        //for (int i = 0; i < attemps; i++)
        //{
        //    string actualText = GetRankText().Text;

        //    if (actualText != "Оценка содержания: 0")
        //    {
        //        return actualText == expectedText;
        //    }

        //    Thread.Sleep(1000);
        //    _webDriver.Navigate().Refresh();
        //}

        //return false;
        int maxAttempts = 5;
        for (int i = 0; i < maxAttempts; i++)
        {
            try
            {
                string actualText = GetRankText().Text;
                if (actualText == expectedText)
                {
                    return true;
                }
                if (!actualText.Contains("Оценка содержания: 0"))
                {
                    return false; // Ранг вычислен, но не совпал
                }
            }
            catch (NoSuchElementException)
            {
                // Элемент не найден
            }

            Thread.Sleep(1000);
            _webDriver.Navigate().Refresh();
        }

        return false;
    }

    public bool IsSimilarityEqualTo(double similarity)
    {
        string expectedText = $"Плагиат: {similarity.ToString(CultureInfo.InvariantCulture)}";
        string actualText = GetSimilarityText().Text;

        return expectedText == actualText;
    }
}