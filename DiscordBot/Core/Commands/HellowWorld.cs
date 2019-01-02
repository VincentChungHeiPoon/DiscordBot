using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;

namespace DiscordBot.Core.Commands
{
    public class HellowWorld : ModuleBase<SocketCommandContext>
    {
        [Command("hello"), Alias("helloworld","world"), Summary("Hello world command")]
        public async Task Hello()
        {
            await Context.Channel.SendMessageAsync("Hello world");
        }

        [Command("embed"), Summary("Enbed test command")]
        public async Task Embed([Remainder] string Input = "None")
        {
            EmbedBuilder Embed = new EmbedBuilder();

            Embed.WithAuthor("Test embed", Context.User.GetAvatarUrl());
            Embed.WithColor(40,200,150);
            Embed.WithFooter("The footer of the embed", Context.Guild.Owner.GetAvatarUrl());
            Embed.WithDescription("Dummy description, with a link \n" +
                "[my favourite website](https://discord.foxbot.me/stable/api/index.html)");
            Embed.AddInlineField("User Input: ", Input);
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
    }
}
