using FlashCardForge.Core.Contracts.Services;
using FlashCardForge.Core.Models;

namespace FlashCardForge.Core.Services;

// This class holds sample data used by some generated pages to show how they can be used.
// TODO: The following classes have been created to display sample data. Delete these files once your app is using real data.
// 1. Contracts/Services/ISampleDataService.cs
// 2. Services/SampleDataService.cs
// 3. Models/SampleCompany.cs
// 4. Models/SampleOrder.cs
// 5. Models/SampleOrderDetail.cs
public class DeckDataService : IDeckDataService
{
    private readonly List<Card> _cards;

    public DeckDataService()
    {
        _cards = new List<Card>();
    }
    public async Task SaveCardAsync(Card card)
    {
        await Task.Run(() => _cards.Add(card));
    }

    public async Task<IEnumerable<Card>> GetGridDataAsync()
    {
        return await Task.FromResult(_cards.AsEnumerable());
    }
}
