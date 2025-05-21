using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace E2ETests.Pages;

public class IndexPage
{
    IWebDriver _webDriver;
    private static readonly By
        _textAreaXPath = By.XPath("//textarea[@name='text']"),
        _regionSelectedXPath = By.XPath("//select[@name='region']"),
        _sumbitBtnXPath = By.XPath("//input[@type='submit']"),
        _regionSelectorOptionsXPath = By.XPath(".//option");

    public IndexPage(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public IWebElement GetTextArea()
    {
        return _webDriver.FindElement(_textAreaXPath);
    }
    public IWebElement GetRegionSelecter()
    {
        return _webDriver.FindElement(_regionSelectedXPath);
    }
    public IWebElement GetSumbitBtn()
    {
        return _webDriver.FindElement(_sumbitBtnXPath);
    }

    public void SetTextToArea(string text)
    {
        GetTextArea().SendKeys(text);
    }
    public void SelectRegion(string text)
    {
        IWebElement selector = GetRegionSelecter();
        selector.Click();

        ReadOnlyCollection<IWebElement> options = selector.FindElements(_regionSelectorOptionsXPath);

        foreach (IWebElement option in options)
        {
            if (option.Text == text)
            {
                option.Click();
                break;
            }
        }
    }
    public void SumbitText()
    {
        GetSumbitBtn().Click();
    }
}