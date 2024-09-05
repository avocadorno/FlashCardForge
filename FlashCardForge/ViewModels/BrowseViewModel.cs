using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper.Configuration;
using CsvHelper;
using FlashCardForge.Contracts.ViewModels;
using FlashCardForge.Core.Contracts.Services;
using FlashCardForge.Core.Models;
using FlashCardForge.Core.Models.ClassMap;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System.Globalization;

namespace FlashCardForge.ViewModels;

public partial class BrowseViewModel : ObservableRecipient, INavigationAware
{
    private readonly IDeckDataService _deckDataService;

    [ObservableProperty]
    private string? _searchTerm;

    public ObservableCollection<Card> Source { get; } = new ObservableCollection<Card>();

    public BrowseViewModel(IDeckDataService sampleDataService)
    {
        _deckDataService = sampleDataService;
    }

    [RelayCommand]
    private async Task ApplySearch()
    {
        Source.Clear();
        var data = await _deckDataService.GetGridDataAsync();
        if (string.IsNullOrEmpty(SearchTerm))
        {
            foreach (var item in data)
            {
                Source.Add(item);
            }
        }
        else
        {
            foreach (var item in data)
            {
                if (item.Word.Contains(SearchTerm))
                    Source.Add(item);
            }
        }
    }

    [RelayCommand]
    public async Task ExportDeck()
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

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        var data = await _deckDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
