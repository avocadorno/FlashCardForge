﻿using System.Globalization;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper.Configuration;
using CsvHelper;
using FlashCardForge.Core.Contracts.Services;
using FlashCardForge.Core.Helpers;
using FlashCardForge.Core.Models.ClassMap;
using FlashCardForge.Core.Models;
using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;

namespace FlashCardForge.ViewModels;

public partial class DefineViewModel : ObservableRecipient
{
    private const int MAX_RETRIES = 10;
    private readonly IWordExtractionService _wordExtractionService;
    private readonly ILemmatizationService _lemmatizationService;

    private readonly List<Card> deck;
    private readonly HtmlDocument _htmlDocument;
    private Action _keywordTextBoxSelectAllAction;

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LookupCommand))]
    private string? _keyword;
    [ObservableProperty]
    private string? _word;
    [ObservableProperty]
    private string? _definition;
    [ObservableProperty]
    private string? _audioURL;
    [ObservableProperty]
    private string? _image;
    [ObservableProperty]
    private string? _deckName;

    public DefineViewModel(IWordExtractionService wordExtractionService, ILemmatizationService lemmatizationService)
    {
        _htmlDocument = new HtmlDocument();
        _wordExtractionService = wordExtractionService;
        _lemmatizationService = lemmatizationService;
        deck = new List<Card>();
    }

    [RelayCommand(CanExecute = nameof(CanLookup))]
    private void Lookup()
    {
        if (!string.IsNullOrEmpty(Keyword))
        {
            _htmlDocument.LoadHtml(_wordExtractionService.GetHtmlString(Keyword));
            UpdateFields();
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Keyword = string.Empty;
        Definition = string.Empty;
        AudioURL = string.Empty;
    }

    private bool CanLookup() => !string.IsNullOrEmpty(Keyword);

    public void SetKeyWordTextBoxSelectAllCommand(Action keywordTextBoxSelectAllAction)
    {
        _keywordTextBoxSelectAllAction = keywordTextBoxSelectAllAction;
    }

    [RelayCommand]
    public void AddToDeck()
    {
        deck.Add(new Card
        {
            Word = Keyword,
            Definition = HTMLHelper.GetBeautified(Definition),
            AudioURL = AudioURL,
            AddedDate = DateTime.Now
        });
        Clear();
    }

    [RelayCommand]
    public async void Export()
    {
        foreach (var card in deck)
        {
            string url = card.AudioURL;
            string filePath = $"D:/Output/{card.AudioFileName}";

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                   fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
            }
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|"
        };

        using (var writer = new StreamWriter("D:/Output/output.csv"))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.Context.RegisterClassMap<CardMap>();
            csv.WriteRecords(deck);
        }
    }

    private async void UpdateFields()
    {
        Keyword = _wordExtractionService.GetWord(_htmlDocument);
        _keywordTextBoxSelectAllAction();
        AudioURL = _wordExtractionService.GetAudioURL(_htmlDocument);
        Definition = _wordExtractionService.GetDefinition(_htmlDocument);
        var lemmas = _lemmatizationService.GetAppearedReflection(Definition, Keyword);
        foreach (var lemma in await lemmas)
            Definition = Definition.Replace(lemma, "____");
    }
}
