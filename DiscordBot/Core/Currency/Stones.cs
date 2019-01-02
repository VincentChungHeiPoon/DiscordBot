using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Discord.Commands;
using Discord;
using Discord.WebSocket;

using DiscordBot.Core.Data;
using DiscordBot.Resources.Database;

namespace DiscordBot.Core.Currency
{
    public class Stones : ModuleBase<SocketCommandContext>
    {
        //shop buy
        [Group("stone"), Alias("stones"), Summary("Group to manage stuff to do with stones")]
        public class StonesGroup : ModuleBase<SocketCommandContext>
        {
            [Command(""), Alias("me", "my"), Summary("Shows all your current stones")]
            public async Task Me(IUser User = null)
            {
                if(User == null)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username}, you have {Data.Data.GetStone(Context.User.Id)} stone");
                }
                else
                {
                    //await Context.Channel.SendMessageAsync("Testing get command");
                    await Context.Channel.SendMessageAsync($"{User.Username}, you have {Data.Data.GetStone(User.Id)} stone");
                }              
            }

            [Command("give"), Alias("gift"), Summary("Used to give people stones")]
            public async Task Give(IUser User = null, int Amount = 0)
            {
                // stone give parameter
                //group cmd user amount 

                //Checks
                //Permission??
                //Enough stones to give?
                if (User == null)
                {
                    //user didnt input
                    await Context.Channel.SendMessageAsync(":x: You didn't mention a user to give the stone to! Please use this syntax >>stone give **<@user>** <amount>");
                    return;
                }

                if (User.IsBot)
                {
                    await Context.Channel.SendMessageAsync(":x: Bots ccant use this bot, so you can't give stones to a bot!");
                    return;
                }

                if (Amount == 0)
                {
                    await Context.Channel.SendMessageAsync($":x: You need to specify a valid amouint of stones that i neede to give to {User.Username}!");
                    return;
                }

                SocketGuildUser User1 = Context.User as SocketGuildUser;

                if(!User1.GuildPermissions.Administrator)
                {
                    await Context.Channel.SendMessageAsync($":x: you don't have the addminisrator permission in this discord server! Ask a moderator ot the owen to execute this command!");
                    return;
                }

                //Exectution
                //Calculation
                await Context.Channel.SendMessageAsync($":tada: {User.Mention} you have received **{Amount}** stones from {Context.User.Username}");

                //saving data
                //sava data to the database
                //save a file
                await Data.Data.SaveStone(User.Id, Amount);
            }

            [Command("reset"), Summary("reset the user's entire progress")]
            public async Task Reset(IUser User = null)
            {
                //checks
                if(User == null)
                {
                    await Context.Channel.SendMessageAsync($":x: You need to tell me which user you want to reset the stones of! For example >>stone reset {Context.User.Username}");
                    return;
                }
                if(User.IsBot)
                {
                    await Context.Channel.SendMessageAsync($":x: Bot can't use this bot, you also can't reset progress of a bot");
                }
                SocketGuildUser User1 = Context.User as SocketGuildUser;
                if (!User1.GuildPermissions.Administrator)
                {
                    await Context.Channel.SendMessageAsync($":x: you don't have the addminisrator permission in this discord server! Ask a moderator ot the owen to execute this command!");
                    return;
                }
                //execution
                await Context.Channel.SendMessageAsync($":skull: {User.Mention}, you have been reset by {Context.User.Username}, This means you have lost all your stones!");
                //save

                using (var DbContext = new SqliteDbContext())
                {
                    //the linq statement is sql, and it return a list.
                    DbContext.Stones.RemoveRange(DbContext.Stones.Where(x => x.UserID == User.Id));
                    await DbContext.SaveChangesAsync();
                }
            }
        }
    }
}
