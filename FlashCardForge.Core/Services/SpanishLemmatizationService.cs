using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalyst;
using FlashCardForge.Core.Contracts.Services;
using Mosaik.Core;

namespace FlashCardForge.Core.Services;
public class SpanishLemmatizationService : ILemmatizationService
{

    private Pipeline _nlp;
    public List<string> GetAppearedReflection(string doc, string word)
    {
        Catalyst.Models.Spanish.Register();
        Storage.Current = new DiskStorage("catalyst-models");
        _nlp = Pipeline.ForAsync(Language.Spanish).GetAwaiter().GetResult();

        List<string> lemmas = new List<string>() { word };
        var catalystDoc = new Document(doc, Language.Spanish);
        _nlp.ProcessSingle(catalystDoc);
        foreach (var sentence in catalystDoc)
        {
            foreach (var token in sentence)
            {
                if (token.Lemma.Equals(word, StringComparison.OrdinalIgnoreCase))
                    lemmas.Add(token.Value);
            }
        }
        var uniqueLemmas = lemmas.Distinct().ToList();
        uniqueLemmas.Sort((x, y) => y.Length.CompareTo(x.Length));
        return uniqueLemmas;
    }
}
