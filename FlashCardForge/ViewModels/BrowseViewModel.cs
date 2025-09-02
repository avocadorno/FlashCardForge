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

    private async Task<bool> DownloadFileAsync(HttpClient client, string url, string filePath)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            using Stream contentStream = await response.Content.ReadAsStreamAsync();
            using FileStream fileStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 8192,
                useAsync: true);

            await contentStream.CopyToAsync(fileStream);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    [RelayCommand]
    public async Task ExportDeck()
    {
        var downloadFolder = @"D:/Output/";
        if (!Directory.Exists(downloadFolder))
        {
            Directory.CreateDirectory(downloadFolder);
        }

        using HttpClient client = new HttpClient();
        var downloadTasks = new List<Task<bool>>();
        foreach (var card in await _deckDataService.GetGridDataAsync())
        {
            var url = card.AudioURL;
            var filePath = $"{downloadFolder}{card.AudioFileName}";
            if (string.IsNullOrEmpty(url))
                continue;
            downloadTasks.Add(DownloadFileAsync(client, url, filePath));
        }

        await Task.WhenAll(downloadTasks);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|"
        };

        var outFileName = "";
        var fileIndex = 0;
        do
        {
            outFileName = $"output_{fileIndex}.csv";
            fileIndex += 1;
        }
        while (File.Exists($"{downloadFolder}{outFileName}"));

        using var writer = new StreamWriter($"{downloadFolder}{outFileName}");
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
