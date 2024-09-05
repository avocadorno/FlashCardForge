using FlashCardForge.Core.Models;

namespace FlashCardForge.Core.Contracts.Services;

public interface IDeckDataService
{
    Task SaveCardAsync(Card card);
    Task<IEnumerable<Card>> GetGridDataAsync();
}
