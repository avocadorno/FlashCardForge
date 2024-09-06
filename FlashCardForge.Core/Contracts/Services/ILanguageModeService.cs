using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardForge.Core.Contracts.Services;
public interface ILanguageModeService
{
    public int SelectedMode { get; set; }
    public List<string> LanguageModes { get; }
}
