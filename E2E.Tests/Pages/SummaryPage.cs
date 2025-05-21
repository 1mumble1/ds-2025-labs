using System.Globalization;
using OpenQA.Selenium;

namespace E2ETests.Pages;

public class SummaryPage
{
    IWebDriver _webDriver;
    private static readonly By
        _rankTextXPath = By.XPath("//p[@id='rank']");

    public SummaryPage(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public IWebElement GetRankText()
    {
        return _webDriver.FindElement(_rankTextXPath);
    }

    public bool IsRankEqualTo(string rank)
    {
        string expectedText = $"Оценка содержания: {rank}";

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
            }
            catch (NoSuchElementException)
            {
                // Элемент не найден
            }

            Thread.Sleep(3000);
            _webDriver.Navigate().Refresh();
        }

        return false;
    }
}