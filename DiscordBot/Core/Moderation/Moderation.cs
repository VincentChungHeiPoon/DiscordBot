using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using Discord.Commands;
using DiscordBot.Resources.Settings;
using DiscordBot.Resources.Datatypes;

using Newtonsoft.Json;

namespace DiscordBot.Core.Moderation
{
    public class Moderation:ModuleBase<SocketCommandContext>
    {
        [Command("reload"), Summary("Reload the settings.json file while the bot is running")]
        public async Task Reload()
        {
            //check
            if (Context.User.Id != ESettings.Owner)
            {
                await Context.Channel.SendMessageAsync(":x: You are not the owner. ask the bot owenr to execute this command");
                return;
            }

            string SettingsLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1", @"Data\Settings.json"));
            if (!File.Exists(SettingsLocation))
            {
                await Context.Channel.SendMessageAsync(":x: The file is not found the in the given location, expected location can be found in the log!");
                Console.WriteLine(SettingsLocation);
                return;
            }
            //execution
            string JSON = "";
            using (var Stream = new FileStream(SettingsLocation, FileMode.Open, FileAccess.Read))
            using (var ReadSettings = new StreamReader(Stream))
            {
                JSON = ReadSettings.ReadToEnd();
            }
            Setting Settings = JsonConvert.DeserializeObject<Setting>(JSON);

            //save
            ESettings.Banned = Settings.banned;
            ESettings.Log = Settings.log;
            ESettings.Owner = Settings.owner;
            ESettings.Token = Settings.token;
            ESettings.Version = Settings.version;

            await Context.Channel.SendMessageAsync(":white_check_mark: All the setting were updated successfully!");
        }
    }
}
