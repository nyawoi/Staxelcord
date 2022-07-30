using System;
using System.Linq;
using System.Threading.Tasks;
using AetharNet.Moonbow.Experimental.Utilities;
using AetharNet.Staxelcord.Config;
using AetharNet.Staxelcord.Hooks;
using AetharNet.Staxelcord.SlashCommands;
using AetharNet.Staxelcord.Utilities;
using Discord;
using Discord.WebSocket;
using Plukit.Base;

namespace AetharNet.Staxelcord.Clients
{
    internal class DiscordBridgeClient
    {
        private static readonly string StaxelChatPrefix = ChatFormat.Format("[Discord]", TextStyling.Bold, "#5865F2");

        private readonly Configuration _configuration;
        private readonly DiscordSocketClient _client;
        private ITextChannel _channel;

        public DiscordBridgeClient(Configuration configuration)
        {
            _configuration = configuration;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = false,
                AlwaysDownloadDefaultStickers = false,
                AlwaysResolveStickers = false,
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages,
                LargeThreshold = 0,
                MessageCacheSize = 0,
                LogGatewayIntentWarnings = true
            });
        }

        public async Task Initialize()
        {
            _client.Log += ClientOnLog;
            _client.Ready += ClientOnReady;
            _client.Disconnected += ClientOnDisconnected;
            _client.MessageReceived += ClientOnMessageReceived;
            _client.SlashCommandExecuted += ClientOnSlashCommandExecuted;

            await _client.LoginAsync(TokenType.Bot, _configuration.Token);
            await _client.StartAsync();
            
            await Task.Delay(-1);
        }

        private Task ClientOnLog(LogMessage message)
        {
            AdminLogHook.AddLog(message.Message);
            return Task.CompletedTask;
        }

        private async Task ClientOnDisconnected(Exception ex)
        {
            if (ex.Message.Contains("task"))
            {
                AdminLogHook.AddLog(ex.Message);
                return;
            }
            
            AdminLogHook.AddLog($"Encountered error while attempting to connect to Discord: {ex}");

            await _client.StopAsync();
            await _client.DisposeAsync();
        }

        public bool IsConnected() => _client.LoginState == LoginState.LoggedIn;

        public void BridgeStaxelMessage(string playerName, string message)
        {
            if (_channel == null) return;

            var cleanedName = DiscordUtilities.EscapeContent(playerName);
            var cleanedMessage = DiscordUtilities.EscapeContent(message);

            _channel.SendMessageAsync($"**{cleanedName}**:\n{cleanedMessage.Split('\n').Select(line => "> " + line).JoinStrings("\n")}");
        }

        private async Task ClientOnReady()
        {
            AdminLogHook.AddLog("READY!");
            
            try
            {
                var sourceChannel = await _client.GetChannelAsync(_configuration.ChannelId);

                if (sourceChannel is ITextChannel textChannel)
                {
                    _channel = textChannel;

                    var commands = await _channel.Guild.GetApplicationCommandsAsync();

                    if (commands.Count == 0)
                    {
                        await _channel.Guild.CreateApplicationCommandAsync(InfoSlashCommand.Build());
                        await _channel.Guild.CreateApplicationCommandAsync(ModsSlashCommand.Build());
                        await _channel.Guild.CreateApplicationCommandAsync(OnlineSlashCommand.Build());
                    }
                    
                    AdminLogHook.AddLog("Setup completed. Please enjoy the experience.");
                }
                else
                {
                    AdminLogHook.AddLog("Could not locate provided bridge channel. Please check Staxelcord configuration and Discord bot permissions.");
                    
                    await _client.LogoutAsync();
                }
            }
            catch (Exception ex)
            {
                AdminLogHook.AddLog($"Encountered error while retrieving bridged channel: {ex.Message}");
                
                await _client.LogoutAsync();
            }
        }
        
        private Task ClientOnMessageReceived(SocketMessage message)
        {
            if (_channel == null
                || _channel.Id != message.Channel.Id
                || message.Author.IsBot
                || message.Author.IsWebhook) return Task.CompletedTask;
            
            var roleColor = Color.LighterGrey;
            var memberRoles = (message.Author as SocketGuildUser)?.Roles;
            
            if (memberRoles != null)
            {
                foreach (var role in memberRoles.OrderBy(role => role.Position))
                {
                    if (role.Color == Color.Default) continue;
                    roleColor = role.Color;
                    break;
                }
            }
            
            var roleColorHex = roleColor.R.ToString("X").PadLeft(2, '0')
                               + roleColor.G.ToString("X").PadLeft(2, '0')
                               + roleColor.B.ToString("X").PadLeft(2, '0');
            
            // Right, so.. you can't have <> in your message; it has to be split up to show up
            var messageAuthor = ChatFormat.Color("<", roleColorHex) + ChatFormat.Color(message.Author.Username, roleColorHex) + ChatFormat.Color(">", roleColorHex);
            
            ServerMessaging.MessageAllPlayersPlainText($"{StaxelChatPrefix} {messageAuthor}: {message.CleanContent}");
            
            return Task.CompletedTask;
        }
        
        private Task ClientOnSlashCommandExecuted(SocketSlashCommand command)
        {
            if (_channel == null || _channel.Id != command.ChannelId) return Task.CompletedTask;

            return command.Data.Name switch
            {
                InfoSlashCommand.CommandName => InfoSlashCommand.Execute(command),
                ModsSlashCommand.CommandName => ModsSlashCommand.Execute(command),
                OnlineSlashCommand.CommandName => OnlineSlashCommand.Execute(command),
                _ => Task.CompletedTask
            };
        }
    }
}