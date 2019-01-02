﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json;

using DiscordBot.Resources.Datatypes;
using DiscordBot.Resources.Settings;
using System.IO;

namespace DiscordBot
{
    //this project's tutorial section include HelloWorld.cs Sticker.cs, Stones.cs, Data.cs, Backdoor.cs, Moderation.cs
    //remaining are written by Vincent Poon
    class Program
    {
        private DiscordSocketClient Client;
        private CommandService Commands;

        private string commandPrefix = ">>";

        //Main is called when app start, begins MainAsync with lambda expression
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        

        private async Task MainAsync()
        {
            string JSON = "";
            string SettingsLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1", @"Data\Settings.json"));
            using (var Stream = new FileStream(SettingsLocation, FileMode.Open, FileAccess.Read))
            using (var ReadSettings = new StreamReader(Stream))
            {
                JSON = ReadSettings.ReadToEnd();
            }
            Setting Settings = JsonConvert.DeserializeObject<Setting>(JSON);

            ESettings.Banned = Settings.banned;
            ESettings.Log = Settings.log;
            ESettings.Owner = Settings.owner;
            ESettings.Token = Settings.token;
            ESettings.Version = Settings.version;


            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,                
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());

            //bot started/stopped
            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            //bot login          
            await Client.LoginAsync(TokenType.Bot, ESettings.Token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
            try
            {
                SocketGuild Guild = Client.Guilds.Where(x => x.Id == ESettings.Log[0]).FirstOrDefault();
                SocketTextChannel Channel = Guild.Channels.Where(x => x.Id == ESettings.Log[1]).FirstOrDefault() as SocketTextChannel;
                Channel.SendMessageAsync($"{DateTime.Now} at {Message.Source}] {Message.Message}");
            }
            catch { };
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("Testing", "https://vincentchungheipoon.github.io/", StreamType.NotStreaming);
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;

            int ArgPos = 0;

            if (!(Message.HasStringPrefix(commandPrefix, ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;

            var Result = await Commands.ExecuteAsync(Context, ArgPos);
            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Command Something went wrong with executing a command. Text {Context.Message.Content}. | Error : {Result.ErrorReason}");
            }
        }
    }
}
