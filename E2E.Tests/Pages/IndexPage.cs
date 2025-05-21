using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace E2ETests.Pages;

public class IndexPage
{
    IWebDriver _webDriver;
    private static readonly By
        _textAreaXPath = By.XPath("//textarea[@name='text']"),
        _submitButtonXPath = By.XPath("//input[@type='submit']");

    public IndexPage(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public IWebElement GetTextArea()
    {
        return _webDriver.FindElement(_textAreaXPath);
    }

    public IWebElement GetSubmitButton()
    {
        return _webDriver.FindElement(_submitButtonXPath);
    }

    public void SetTextToArea(string text)
    {
        GetTextArea().SendKeys(text);
    }
    public void Submit()
    {
        GetSubmitButton().Click();
    }
}