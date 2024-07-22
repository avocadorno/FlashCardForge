﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardForge.Core.Models;
internal class Deck
{
    [Key]
    public Guid Id
    {
        get; set;
    }
    public string Name
    {
        get; set;
    }
    public Language Language
    {
        get; set;
    }
    public ICollection<Card> Cards
    {
        get; set;
    }
}
