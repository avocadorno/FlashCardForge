using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashCardForge.Core.Contracts.Services;

namespace FlashCardForge.Core.Services;
public class LanguageModeService : ILanguageModeService
{
    private int _languageMode;
    private readonly List<string> _languages = new() { "Spanish", "Italian" };

    public int SelectedMode
    {
        get
        {
            return _selectedMode;
        }
        set
        {
            if (value >= 0 && value < _languageModes.Count)
            {
                _selectedMode = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value), "SelectedMode must be within the range of available language modes.");
            }
        }
    }

    public List<string> LanguageModes => _languages;
}
