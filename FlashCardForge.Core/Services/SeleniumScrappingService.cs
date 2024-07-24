using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashCardForge.Core.Contracts.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace FlashCardForge.Core.Services;
internal class SeleniumScrappingService : IWebScrappingService
{
    private IWebDriver _driver;
    private IWebDriver WebDriver
    {
        get
        {
            if (_driver == null)
            {
                var options = new ChromeOptions();
                options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
                _driver = new ChromeDriver(options);
            }
            return _driver;
        }
    }

    public string ScrapeWebsite(string url, string buttonCssSelector, string waitForElementCssSelector)
    {
        WebDriver.Navigate().GoToUrl(url);
        var button = WebDriver.FindElement(By.CssSelector(buttonCssSelector));
        button.Click();
        var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10));
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(waitForElementCssSelector)));
        return WebDriver.PageSource;
    }

    public string ScrapeWebsite(string url)
    {
        WebDriver.Navigate().GoToUrl(url);
        return WebDriver.PageSource;
    }
}
