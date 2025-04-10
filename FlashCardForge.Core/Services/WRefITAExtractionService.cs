﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Common;
using FlashCardForge.Core.Contracts.Services;
using FlashCardForge.Core.Helpers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace FlashCardForge.Core.Services;
public class WRefITAExtractionService : IWordExtractionService
{
    private readonly IWebScrappingService _scrappingService;
    private readonly ILemmatizationService _lemmatizationService;

    public WRefITAExtractionService()
    {
        _scrappingService = new SeleniumScrappingService();
        _lemmatizationService = new LemmatizationService(Mosaik.Core.Language.Italian);
    }

    public string BaseURL => "https://www.wordreference.com";

    public string GetQueryURL(string keyword) => $"{BaseURL}/iten/{keyword}";

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
        IList<HtmlNode> gramCats = htmlDocument.QuerySelectorAll("#clickableHC .gramcat");

        foreach (var gramCat in gramCats)
        {
            var partOfSpeech = "";
            try
            {
                var temp = gramCat.QuerySelector(".pos");
                if (temp is null)
                    return string.Empty;

                partOfSpeech = PSAbbreviationHelper.GetFullPS(temp.InnerText.Trim(), PSAbbreviationHelper.Language.Italian);
            }
            catch (Exception)
            {
                return string.Empty;
            }
            definitionText += HTMLHelper.GetWrapped(partOfSpeech, "i") + "\n";
            var senses = gramCat.QuerySelectorAll(".senses > .sense");
            if (senses.Count == 0)
                senses = gramCat.QuerySelectorAll(".sense");
            var definitions = new List<string>();
            foreach (var sense in senses)
            {
                HTMLHelper.RemoveTag(sense, ".ps");
                var italics = new List<string> {".subjarea", ".lbmisc" };
                HTMLHelper.ReplaceTag(sense, italics, "i");

                var senseHtml = sense.InnerHtml;

                var parentDiv = sense;
                var firstPhraseSpan = parentDiv.SelectSingleNode("//*[contains(@class, 'phrasegroup') or contains(@class, 'phrasegroup') or contains(@class, 'compoundgroup')]");
                var contentBeforePhrase = string.Empty;
                foreach (var node in parentDiv.ChildNodes)
                {
                    if (node == firstPhraseSpan)
                        break;

                    if (node.Name != "br")
                    {
                        HTMLHelper.ReplaceTag(node, italics, "i");
                        contentBeforePhrase += (node.Name == "span") ? node.InnerText : node.OuterHtml;
                    }
                }

                var definition = HTMLHelper.GetBold(contentBeforePhrase);

                var phraseSpans = parentDiv.SelectNodes("//*[contains(@class, 'phrasegroup') or contains(@class, 'phrasegroup') or contains(@class, 'compoundgroup')]")?.ToList();

                List<string> phraseList = new List<string>();
                if (phraseSpans != null)
                {
                    foreach (var node in phraseSpans)
                    {
                        var exampleList = node.QuerySelectorAll(".phrase");
                        if (exampleList.Count == 0)
                            exampleList = node.QuerySelectorAll(".compound");
                        var phrase = string.Join(" ", exampleList.Select(example => example.InnerText).ToList());
                        var tranList = node.QuerySelectorAll(".tran");
                        var tran = string.Join(" ", tranList.Select(tran => tran.InnerText).ToList());
                        phraseList.Add(phrase + ": " + HTMLHelper.GetItalic(tran));
                    }
                }
                var phrases = (phraseList.Count > 0) ? HTMLHelper.GetUnOrderedList(phraseList) : String.Empty;
                definitions.Add(definition + "\n" + phrases);

            }
            definitionText += (definitions.Count > 0) ? HTMLHelper.GetOrderedList(definitions) + "<hr>" : String.Empty;
        }
        var tagToRemove = "<hr>";

        var lastIndex = definitionText.LastIndexOf(tagToRemove);

        if (lastIndex >= 0)
        {
            definitionText = definitionText.Remove(lastIndex, tagToRemove.Length);
        }
        return HTMLHelper.GetBeautified(definitionText);
    }

    public async Task<string> GetMaskedDefinition(HtmlDocument htmlDocument, string Keyword)
    {
        var definition = GetDefinition(htmlDocument);
        var lemmas = _lemmatizationService.GetAppearedReflection(definition, Keyword);
        foreach (var lemma in await lemmas)
        {
            if (!string.IsNullOrEmpty(lemma))
            {
                var pattern = $@"\b{lemma}\b";
                definition = Regex.Replace(definition, pattern, $"#{lemma}#");
            }
        }

        return definition;
    }


    public string GetAudioURL(HtmlDocument htmlDocument)
    {
        var audioString = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"listen_widget\"]/script")?.InnerHtml;
        if (audioString == null)
            return string.Empty;
        var pattern = @"\/audio\/it\/it\/it\d+\.mp3";
        Match match = Regex.Match(audioString, pattern);
        return match.Success ? BaseURL + match.Value : string.Empty;
    }
    public string GetPronunciation(HtmlDocument htmlDocument)
    {
        var pronNode = htmlDocument.QuerySelector(".pron");
        if (pronNode == null)
            return string.Empty;
        return $"[{pronNode.InnerHtml}]";
    }
}
