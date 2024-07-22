using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using FlashCardForge.Contracts.ViewModels;
using FlashCardForge.Core.Contracts.Services;
using FlashCardForge.Core.Models;

namespace FlashCardForge.ViewModels;

public partial class BrowseViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public BrowseViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await _sampleDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
