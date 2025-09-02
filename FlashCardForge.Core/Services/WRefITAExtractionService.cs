using System;
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
    private readonly String _cssStyle = "<style>.phonetics{color:var(--body-highlighted-text-color);font-weight:700;font-family:'Lucida Sans Unicode','Arial Unicode MS','Lucida Grande'}.CA,.CC,.CH,.CN,.CO,.CS,.CV,.GR,.IN,.LF,.NC,.RN,.RR,.SF,.SN,.box.ex,.inflecter,.ital,.prov,.register,.sbox,.translation.gloss,.xr{font-style:italic}.CA,.CC,.CH,.CN,.CO,.CS,.CV,.GR,.IN,.LF,.NC,.RN,.RR,.SF,.SN,.gr,.inflecter,.prov,.register,.translation.gloss{color:var(--body-highlighted-text-color)}.HWexpansion,.aux,.bold,.catnumber,.hw,.reverse,.AF,.FF,.box.ex,.inflected,.label.ff,.phrase,.headnumber,.prep.orig,.xr a,.xrsense{font-weight:700!important}.hw{display:block}.hw:not(:first-of-type):not(.hwextension):not(.hwaltform){margin-top:2em}.BOXL{display:block;border:1px solid #000;margin-top:10px;padding:5px}.uline{text-decoration:underline}.or{font-style:italic;color:var(--body-highlighted-text-color);font-weight:700}.hw .or{font-weight:400}.phrase{font-weight:700}.phrase.example{font-style:normal}.phrase.example.inverted{margin-left:auto}.LXRN,.LXSF,.box.register,.upper{font-variant:small-caps}.TRexpansion{font-style:italic}.translation,.udef{font-family:sans-serif;margin-left:15px}.translation.register{margin-left:auto}.nomargin{margin-left:auto}.ps{font-family:sans-serif;color:var(--body-highlighted-text-color);font-weight:400}.category{margin-left:15px;margin-top:8px}.catsecondary{margin-left:30px}.third{margin-left:45px}.misc{border:1px solid #000;padding:0 5px}.xr{display:block}.xr:before{content:'➔'}.xreq,a.xr{display:inline}a .xrsense{text-decoration:none}.headnumber,.xrsense{vertical-align:super}.headnumber{position:absolute;margin-left:-15px}.headnumber10{margin-left:-23px}.third .headnumber{margin-left:-22px}span.normalized{font-weight:400;font-style:normal}.fem{font-weight:400}.sbox,div.box{padding:10px;margin:5px;border:1px solid #222;border-radius:3px}a.footnote{text-decoration:none}</style>";

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
        var definition = new List<string>();
        IList<HtmlNode> categories;
        var categoryNodes = htmlDocument.QuerySelectorAll("#clickableHC > .category");

        if (categoryNodes.Count > 0 && htmlDocument.QuerySelectorAll("#clickableHC > .catsecondary").Count == 0)
            categories = categoryNodes;
        else
            categories = htmlDocument.QuerySelectorAll("#clickableHC");

        foreach (var category in categories)
        {
            definition.Add(category.OuterHtml);
        }
        if (definition.Any())
            definition.Add(_cssStyle);
        return String.Join("\n", definition);
    }
    public async Task<string> GetMaskedDefinition(HtmlDocument htmlDocument, string Keyword)
    {
        var definition = GetDefinition(htmlDocument);
        var lemmas = _lemmatizationService.GetAppearedReflection(definition, Keyword);
        foreach (var lemma in await lemmas)
        {
            if (!string.IsNullOrEmpty(lemma))
            {
                var pattern = $@"(?<!#)\b({Regex.Escape(lemma)})(\w*)(?!#)";
                definition = Regex.Replace(definition, pattern, m => $"#{m.Groups[1].Value}#{m.Groups[2].Value}");
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
