using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardForge.Core.Helpers;
public class PSAbbreviationHelper
{
    private static readonly Dictionary<string, string> _dictionary = new()
    {
        ["sf"] = "sustantivo feminino",
        ["sm"] = "sustantivo masculino",
        ["sm/f"] = "sustantivo masculino/feminino",
        ["smf"] = "sustantivo masculino/feminino",
        ["vt"] = "verbo transitivo",
        ["vi"] = "verbo intransitivo",
        ["vpr"] = "verbo  pronominal",
        ["adv"] = "adverbio",
        ["adj"] = "adjectivo",
        ["n"] = "sustantivo",
        ["conj"] = "conjunción",
    };

    public static string GetFullPS(string abbr) => _dictionary.GetValueOrDefault(abbr.ToLower(), abbr).ToUpper();
}