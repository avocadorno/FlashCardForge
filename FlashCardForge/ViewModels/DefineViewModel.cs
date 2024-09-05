using System.Globalization;
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

    private readonly IDeckDataService _deckDataService;
    private readonly HtmlDocument _htmlDocument;
    private Action? _keywordTextBoxSelectAllAction;

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LookupCommand))]
    private string? _keyword;
    [ObservableProperty]
    private string? _word;
    [ObservableProperty]
    private string? _definition;
    [ObservableProperty]
    private string? _phonetics;
    [ObservableProperty]
    private string? _audioURL;
    [ObservableProperty]
    private string? _image;
    [ObservableProperty]
    private string? _deckName;

    public DefineViewModel(IWordExtractionService wordExtractionService, IDeckDataService deckDataService)
    {
        _htmlDocument = new HtmlDocument();
        _wordExtractionService = wordExtractionService;
        _deckDataService = deckDataService;
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
        Phonetics = string.Empty;
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
        if (!CanAddToDeck())
            return;
        _deckDataService.SaveCardAsync(new Card
        {
            Word = Keyword,
            Definition = HTMLHelper.GetBeautified(Definition),
            Phonetics = Phonetics,
            AudioURL = AudioURL,
            AddedDate = DateTime.Now
        });
        Clear();
    }

    private bool CanAddToDeck() => !string.IsNullOrEmpty(Keyword) && !string.IsNullOrEmpty(Definition);

    [RelayCommand]
    public async Task Export()
    {
        foreach (var card in await _deckDataService.GetGridDataAsync())
        {
            var url = card.AudioURL;
            var filePath = $"D:/Output/{card.AudioFileName}";
            if (string.IsNullOrEmpty(url))
                continue;
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            using Stream contentStream = await response.Content.ReadAsStreamAsync(),
                           fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            await contentStream.CopyToAsync(fileStream);
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|"
        };

        using var writer = new StreamWriter("D:/Output/output.csv");
        using var csv = new CsvWriter(writer, config);
        csv.Context.RegisterClassMap<CardMap>();
        csv.WriteRecords(await _deckDataService.GetGridDataAsync());
    }

    private async void UpdateFields()
    {
        Keyword = _wordExtractionService.GetWord(_htmlDocument);
        _keywordTextBoxSelectAllAction!();
        Phonetics = _wordExtractionService.GetPronunciation(_htmlDocument);
        AudioURL = _wordExtractionService.GetAudioURL(_htmlDocument);
        Definition = await _wordExtractionService.GetMaskedDefinition(_htmlDocument, Keyword);
    }
}
