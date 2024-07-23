using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardForge.Core.Contracts.Services;
public interface ILemmatizationService
{
    public Task<List<string>> GetAppearedReflection(string doc, string word);
}
