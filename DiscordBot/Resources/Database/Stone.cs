using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Resources.Database
{
    public class Stone
    {
        [Key]
        public ulong UserID { get; set; }
        public int Amount { get; set; }
    }
}
