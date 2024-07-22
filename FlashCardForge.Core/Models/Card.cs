using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlashCardForge.Core.Models;
public class Card
{
    [Key]
    public Guid Id
    {
        get; set;
    }
    public string Word
    {
        get; set;
    }
    public string Definition
    {
        get; set;
    }
    public string AudioURL
    {
        get; set;
    }
    public string Phonetics
    {
        get; set;
    }
    public DateTime AddedDate
    {
        get; set;
    }
    public DateTime ExportedDate
    {
        get; set;
    }

    public string AudioFileName
    {
        get
        {
            var pattern = @".*/([^/]+)$";
            Match match = Regex.Match(AudioURL, pattern);
            return (match.Success) ? match.Groups[1].Value : string.Empty;
        }
    }

    public string AudioField => $"[sound:{AudioFileName}]";
}
