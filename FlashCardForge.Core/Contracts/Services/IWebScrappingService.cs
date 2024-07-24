using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardForge.Core.Contracts.Services;
internal interface IWebScrappingService
{
    public string ScrapeWebsite(string url, string buttonCssSelector, string waitForElementCssSelector);
}
