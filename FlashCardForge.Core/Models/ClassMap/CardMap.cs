using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace FlashCardForge.Core.Models.ClassMap;
public sealed class CardMap : ClassMap<Card>
{
    public CardMap()
    {
        Map(c => c.Word);
        Map(c => c.Definition);
        Map(c => c.Phonetics);
        Map(c => c.AudioField);
    }
}
