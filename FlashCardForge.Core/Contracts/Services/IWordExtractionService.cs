﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace FlashCardForge.Core.Contracts.Services;
public interface IWordExtractionService
{
    public string GetQueryURL(string keyword);
    public string BaseURL
    {
        get;
    }
    public string GetHtmlString(string keyword);
    public string GetWord(HtmlDocument htmlDocument);
    public string GetDefinition(HtmlDocument htmlDocument);
    public Task<string> GetMaskedDefinition(HtmlDocument htmlDocument, string Keyword);
    public string GetAudioURL(HtmlDocument htmlDocument);
    public string GetPronunciation(HtmlDocument htmlDocument);
}
