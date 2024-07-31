using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalyst;
using FlashCardForge.Core.Contracts.Services;
using Mosaik.Core;

namespace FlashCardForge.Core.Services;
public class LemmatizationService : ILemmatizationService
{

    private Pipeline _nlp;
    private readonly Language _language;

    public LemmatizationService(Language language)
    {
        _language = language;
        switch (_language)
        {
            case Language.Spanish:
                Catalyst.Models.Spanish.Register();
                break;
            case Language.Italian:
                Catalyst.Models.Italian.Register();
                break;
            default:
                throw new NotSupportedException($"Language {_language} is not supported.");
        }
    }

    public async Task<List<string>> GetAppearedReflection(string doc, string word)
    {
        Storage.Current = new DiskStorage("C:/catalyst-models");
        _nlp = await Pipeline.ForAsync(_language);
        List<string> lemmas = new List<string>() { word };
        var catalystDoc = new Document(doc, _language);
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
