using System.Linq;
using System.Threading.Tasks;
using AetharNet.Moonbow.Experimental.Utilities;
using AetharNet.Staxelcord.Utilities;
using Discord;
using Discord.WebSocket;
using Plukit.Base;
using Staxel.Logic;

namespace AetharNet.Staxelcord.SlashCommands
{
    public class OnlineSlashCommand
    {
        public const string CommandName = "online";

        public static SlashCommandProperties Build()
        {
            return new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("List all online players")
                .Build();
        }

        public static Task Execute(SocketSlashCommand command)
        {
            var players = new Lyst<Entity>();
            GameUtilities.Universe.GetPlayers(players);

            var onlineCountMessage = DiscordUtilities.GetOnlineMessage(players.Count);
            var onlinePlayerList = players.Select(player => "> " + player.PlayerEntityLogic.DisplayName()).JoinStrings("\n");

            return command.RespondAsync($"{onlineCountMessage}\n{onlinePlayerList}");
        }
    }
}