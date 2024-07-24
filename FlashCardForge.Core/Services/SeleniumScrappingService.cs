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
            if (_driver == null || IsBrowserClosed())
            {
                var options = new ChromeOptions();
                options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
                _driver = new ChromeDriver(options);
            }
            return _driver;
        }
    }

    private bool IsBrowserClosed()
    {
        try
        {
            var url = _driver.Url;
            return false;
        }
        catch (WebDriverException)
        {
            return true;
        }
    }

    public string ScrapeWebsite(string url, string buttonCssSelector = null, string waitForElementCssSelector = null)
    {
        try
        {
            WebDriver.Navigate().GoToUrl(url);

            if (buttonCssSelector != null)
            {
                try
                {
                    var button = WebDriver.FindElement(By.CssSelector(buttonCssSelector));
                    button.Click();
                }
                catch (NoSuchElementException)
                {
                    return string.Empty;
                }
            }
            if (waitForElementCssSelector != null)
            {
                try
                {
                    var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(waitForElementCssSelector)));
                }
                catch (WebDriverTimeoutException)
                {
                    return string.Empty;
                }
            }
            return WebDriver.PageSource;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }
}
