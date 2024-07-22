using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashCardForge.Core.Contracts.Services;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace FlashCardForge.Core.Services;
public class CollinsESPExtractionService : IWordExtractionService
{
    public string BaseURL => "https://www.collinsdictionary.com/dictionary/spanish-english/";

    public string GetAudioURL(HtmlDocument htmlDocument) => throw new NotImplementedException();
    public string GetDefinition(HtmlDocument htmlDocument) => throw new NotImplementedException();
    public string GetPronunciation(HtmlDocument htmlDocument) => throw new NotImplementedException();
    public string GetQueryURL(string keyword) => $"https://www.collinsdictionary.com/search/?dictCode=spanish-english&q={keyword}";
    public string GetWord(HtmlDocument htmlDocument) => htmlDocument.QuerySelector(".title_container .orth").InnerText;
}
