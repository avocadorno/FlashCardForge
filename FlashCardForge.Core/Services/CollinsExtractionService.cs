using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashCardForge.Core.Contracts.Services;
using FlashCardForge.Core.Helpers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace FlashCardForge.Core.Services;
public class CollinsExtractionService : IWordExtractionService
{
    private readonly string EntrySectionSelector = ".dictlink > .dictionary.benedict";
    private readonly List<string> ExampleTag = new() { "type-example", "type-idm", "type-phr", "type-lexstring" };
    public string BaseURL => "https://www.collinsdictionary.com/dictionary/spanish-english/";

    public string GetAudioURL(HtmlDocument htmlDocument)
    {
        var entry = htmlDocument.QuerySelector(EntrySectionSelector);
        return entry?.QuerySelectorAll(".hwd_sound.sound").LastOrDefault()?.GetAttributeValue("data-src-mp3", string.Empty) ?? string.Empty;
    }
    public string GetDefinition(HtmlDocument htmlDocument)
    {
        var homs = htmlDocument.QuerySelectorAll(EntrySectionSelector + " > .content .hom");
        var definition = new List<string>();
        foreach (var hom in homs)
        {
            var pos = hom.QuerySelector(".pos")?.InnerText ?? string.Empty;
            var senses = hom.QuerySelectorAll(".gramGrp > .sense").Count() > 0 ? hom.QuerySelectorAll(".gramGrp > .sense") : hom.QuerySelectorAll("> .sense");
            foreach (var sense in senses)
            {
                HTMLHelper.RemoveTag(sense, ".sensenum");
                //TODO: pasar, camino
                var meaning = new List<HtmlNode>();
                var examples = new List<HtmlNode>();
                var exampleSection = false;
                foreach (var child in sense.ChildNodes)
                {
                    if (child.Attributes["class"] != null && ExampleTag.Any(child.Attributes["class"].Value.Contains))
                    {
                        exampleSection = true;
                        examples.Add(child);
                    }
                    if (!exampleSection)
                        meaning.Add(child);
                }
                var meaningText = HTMLHelper.GetWrapped(HTMLHelper.GetBold(string.Join("", meaning.Select(element => element.InnerHtml))), "div");
                var exampleText = HTMLHelper.GetUnOrderedList(examples.Select(example => example.OuterHtml).ToList());
                definition.Add(meaningText + "\n" + exampleText);
            }
        }

        return string.Join("<hr>", definition);
    }

    public string GetHtmlString(string keyword) => throw new NotImplementedException();
    public Task<string> GetMaskedDefinition(HtmlDocument htmlDocument, string Keyword) => throw new NotImplementedException();
    public string GetPronunciation(HtmlDocument htmlDocument) => string.Empty;
    public string GetQueryURL(string keyword) => $"https://www.collinsdictionary.com/search/?dictCode=spanish-english&q={keyword}";
    public string GetWord(HtmlDocument htmlDocument)
    {
        return htmlDocument.DocumentNode.QuerySelector(".title_container .orth")?.InnerText ?? "Not Found";
    }
}
