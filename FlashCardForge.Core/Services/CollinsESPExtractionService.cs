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
    private static readonly string EntrySectionSelector = ".dictlink > .dictionary.benedict";
    public string BaseURL => "https://www.collinsdictionary.com/dictionary/spanish-english/";

    public string GetAudioURL(HtmlDocument htmlDocument)
    {
        var entry = htmlDocument.QuerySelector(EntrySectionSelector);
        try
        {
            return entry.QuerySelectorAll(".hwd_sound.sound").Last().GetAttributeValue("data-src-mp3", string.Empty);
        }
        catch (NullReferenceException)
        {
            return string.Empty;
        }
    }
    public string GetDefinition(HtmlDocument htmlDocument)
    {
        var entry = htmlDocument.QuerySelector(EntrySectionSelector);
        var homs = entry.QuerySelectorAll(".hom");
        var definition = new List<string>();
        foreach (var hom in homs)
        {
            definition.Add(hom.OuterHtml);
        }
        return string.Join("<hr>", definition);
    }
    public string GetPronunciation(HtmlDocument htmlDocument) => string.Empty;
    public string GetQueryURL(string keyword) => $"https://www.collinsdictionary.com/search/?dictCode=spanish-english&q={keyword}";
    public string GetWord(HtmlDocument htmlDocument)
    {
        try
        {
            return htmlDocument.DocumentNode.QuerySelector(".title_container .orth").InnerText;
        }
        catch (NullReferenceException)
        {
            return "Not Found";
        }
    }
}
