using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using FlashCardForge.Contracts.ViewModels;
using FlashCardForge.Core.Contracts.Services;
using FlashCardForge.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

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
