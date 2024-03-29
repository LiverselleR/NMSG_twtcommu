using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Net;
using Newtonsoft.Json;
using Discord.Interactions;

namespace NMSGDiscordBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private readonly CommandService _commands;
        private readonly InteractionService _interactionCommands;
        private readonly IServiceProvider _services;
        private readonly CommandHandler _commandHandler;
        private readonly SlashCommandHandler _slashCommandHandler;

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
            });

            _interactionCommands = new InteractionService(_client, new InteractionServiceConfig
            {
                LogLevel = LogSeverity.Info,
            });
            _commandHandler = new CommandHandler(_client, _commands);
            _slashCommandHandler = new SlashCommandHandler(_client, _interactionCommands);

            _client.Log += Log;
            _commands.Log += Log;
        }

        public async Task MainAsync()
        {
            JSONManager.Initialize();
            // Centralize the logic for commands into a separate method.
            await _commandHandler.InstallCommandsAsync(_services);
            await _slashCommandHandler.InitializeAsync(_services);

            _client.Ready += ReadyAsync;

            // Login and connect.
            await _client.LoginAsync(TokenType.Bot, GetDiscordToken());
            await _client.StartAsync();

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
            
        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message} {msg.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        }

        private string GetDiscordToken()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"/Data";
            
            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"/token.txt";
            using (FileStream fs = File.OpenRead(path))
            {
                StreamReader sr = new StreamReader(fs);
                return sr.ReadToEnd();
            }
        }
        private async Task ReadyAsync()
        {
            await _interactionCommands.RegisterCommandsToGuildAsync(537274953800744961);
        }
    }
}
