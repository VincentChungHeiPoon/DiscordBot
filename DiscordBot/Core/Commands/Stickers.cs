using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using DiscordBot.Core.Data;
using DiscordBot.Resources.Datatypes;

namespace DiscordBot.Core.Commands
{
    public class Stickers : ModuleBase<SocketCommandContext>
    {
        [Command ("getsticker"), Summary("used to get a random sicker from the xml file")]
        public async Task GetSticker()
        {
            Sticker Generated= Data.Data.GetSticker();
            if(Generated == null)
            {
                await Context.Channel.SendMessageAsync(":x: I couldn't find the sticker file :frowing:");
            }

            //now file exists, and we can send the sticker
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor($"Here is your sticker - {Generated.name}");
            Embed.WithImageUrl(Generated.file);
            Embed.WithFooter(Generated.description);

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
    }
}
