using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardForge.Core.Helpers;
public class PSAbbreviationHelper
{
    public enum Language
    {
        Spanish,
        Italian
    }
    
    private static readonly Dictionary<string, string> _ESDictionary = new()
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

    private static readonly Dictionary<string, string> _ITDictionary = new()
    {
        ["sf"] = "sostantivo femminile",
        ["sm"] = "sostantivo maschile",
        ["vt"] = "verbo transitivo",
        ["vi"] = "verbo intransitivo",
        ["avv"] = "avverbio",
        ["agg"] = "aggettivo",
        ["n"] = "sostantivo",
        //TODO
        ["sm/f"] = "sustantivo masculino/feminino",
        ["smf"] = "sustantivo masculino/feminino",
        ["vpr"] = "verbo  pronominal",
        ["conj"] = "conjunción",
    };

    public static string GetFullPS(string abbr, Language language)
    {
        switch (language)
        {
            case Language.Spanish:
                return _ESDictionary.GetValueOrDefault(abbr.ToLower(), abbr).ToUpper();
            case Language.Italian:
                return _ITDictionary.GetValueOrDefault(abbr.ToLower(), abbr).ToUpper();
            default:
                return abbr;
        }
    } 
}