using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FlashCardForge.Core.Contracts.Services;
using FlashCardForge.Core.Helpers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace FlashCardForge.Core.Services;
public class WRefESPExtractionService : IWordExtractionService
{
    private readonly IWebScrappingService _scrappingService;

    public WRefESPExtractionService()
    {
        _scrappingService = new SeleniumScrappingService();
    }

    public string BaseURL => "https://www.wordreference.com";

    public string GetQueryURL(string keyword) => $"{BaseURL}/es/en/translation.asp?spen={keyword}";

    public string GetHtmlString(string keyword)
    {
        return _scrappingService.ScrapeWebsite(GetQueryURL(keyword), "#tabHC", "#clickableHC");
    }

    public string GetWord(HtmlDocument htmlDocument)
    {
        return htmlDocument.QuerySelector(".clickableHC .hw")?.InnerHtml ?? "Not Found";
    }
    public string GetDefinition(HtmlDocument htmlDocument)
    {
        var definitionText = "";
        IList<HtmlNode> categories;
        var categoryNodes = htmlDocument.QuerySelectorAll("#clickableHC > .category");

        if (categoryNodes.Count > 0 && htmlDocument.QuerySelectorAll("#clickableHC > .catsecondary").Count == 0)
            categories = categoryNodes;
        else
            categories = htmlDocument.QuerySelectorAll("#clickableHC");

        foreach (var category in categories)
        {
            HTMLHelper.RemoveTag(category, ".headnumber");
            HTMLHelper.RemoveTag(category, ".xr");
            var partOfSpeech = "";
            try
            {
                var temp = category.QuerySelector(".ps");
                if (temp is null)
                    return string.Empty;

                partOfSpeech = PSAbbreviationHelper.GetFullPS(temp.InnerText.Trim(), PSAbbreviationHelper.Language.Spanish);
                HtmlNode nodeWithClassPs = null;
                foreach (var node in htmlDocument.DocumentNode.DescendantsAndSelf())
                {
                    if (node.HasClass("ps"))
                    {
                        nodeWithClassPs = node;
                        break;
                    }
                }

                if (nodeWithClassPs != null)
                {
                    var nodesToRemove = new List<HtmlNode>();
                    foreach (var node in htmlDocument.DocumentNode.DescendantsAndSelf())
                    {
                        if (node == nodeWithClassPs)
                            break;
                        nodesToRemove.Add(node);
                    }

                    foreach (var node in nodesToRemove)
                        node.Remove();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            definitionText += HTMLHelper.GetWrapped(partOfSpeech, "i") + "\n";
            var catsecondaries = category.QuerySelectorAll(".catsecondary");
            if (catsecondaries.Count == 0)
                catsecondaries = new List<HtmlNode>() { category };
            var definitions = new List<string>();
            foreach (var catsecondary in catsecondaries)
            {
                HTMLHelper.RemoveTag(catsecondary, ".ps");
                var italics = new List<string> { ".SN", ".CO", ".SF", ".IN", ".RN", ".CS", ".ital", ".CN", ".NC", ".register" };
                HTMLHelper.ReplaceTag(catsecondary, italics, "i");
                var underlines = new List<string> { ".uline" };
                HTMLHelper.ReplaceTag(catsecondary, underlines, "u");
                var bolds = new List<string> { ".or", "strong .idiom" };
                HTMLHelper.ReplaceTag(catsecondary, bolds, "b");

                var catsecondaryHtml = catsecondary.InnerHtml;

                var parentDiv = catsecondary;
                var firstPhraseSpan = parentDiv.QuerySelector(".phrase");
                var contentBeforePhrase = string.Empty;
                foreach (var node in parentDiv.ChildNodes)
                {
                    if (node == firstPhraseSpan)
                        break;

                    if (node.Name != "br")
                    {
                        HTMLHelper.ReplaceTag(node, italics, "i");
                        contentBeforePhrase += node.OuterHtml ?? node.InnerText;
                    }
                }

                var definition = HTMLHelper.GetBold(contentBeforePhrase);

                var phraseSpans = parentDiv.QuerySelectorAll(".phrase");

                List<string> partsBetweenBr = new List<string>();
                string contentBetweenBr = string.Empty;
                bool insideBr = false;

                foreach (var node in parentDiv.ChildNodes)
                {
                    if (node.Name == "br")
                    {
                        if (insideBr && !string.IsNullOrEmpty(contentBetweenBr))
                        {
                            partsBetweenBr.Add(contentBetweenBr);
                            contentBetweenBr = string.Empty;
                        }
                        insideBr = false;
                        continue;
                    }

                    if (node.Name == "span" && node.Attributes["class"]?.Value.Contains("phrase") == true)
                    {
                        insideBr = true;
                    }

                    if (insideBr)
                    {
                        contentBetweenBr += node.OuterHtml ?? node.InnerText;
                    }
                }

                // Add the last collected content if any
                if (!string.IsNullOrEmpty(contentBetweenBr))
                {
                    partsBetweenBr.Add(contentBetweenBr);
                }

                var phraseList = partsBetweenBr.Select(phrase =>
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(phrase);

                    var phraseSpan = doc.DocumentNode.QuerySelector(".phrase");

                    StringBuilder resultHtml = new StringBuilder();

                    foreach (var node in doc.DocumentNode.ChildNodes)
                    {
                        if (node == phraseSpan)
                        {
                            resultHtml.Append(node.InnerHtml);
                        }
                        else if (node.NodeType == HtmlNodeType.Text)
                        {
                            resultHtml.Append(HTMLHelper.GetItalic(node.InnerHtml));
                        }
                        else
                        {
                            resultHtml.Append(node.OuterHtml);
                        }
                    }
                    return resultHtml.ToString();
                }).ToList();

                var phrases = (partsBetweenBr.Count > 0) ? HTMLHelper.GetUnOrderedList(phraseList) : String.Empty;
                definitions.Add(definition + "\n" + phrases);
            }
            definitionText += (definitions.Count > 0) ? HTMLHelper.GetOrderedList(definitions) + "<hr>" : String.Empty;
        }
        var tagToRemove = "<hr>";

        int lastIndex = definitionText.LastIndexOf(tagToRemove);

        if (lastIndex >= 0)
        {
            definitionText = definitionText.Remove(lastIndex, tagToRemove.Length);
        }
        return HTMLHelper.GetBeautified(definitionText);
    }


    public string GetAudioURL(HtmlDocument htmlDocument)
    {
        var audioString = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"listen_widget\"]/script")?.InnerHtml;
        if (audioString == null)
            return string.Empty;
        var pattern = @"\/audio\/es\/Castellano\/es\d+\.mp3";
        Match match = Regex.Match(audioString, pattern);
        return match.Success ? BaseURL + match.Value : string.Empty;
    }
    public string GetPronunciation(HtmlDocument htmlDocument) => string.Empty;
}
