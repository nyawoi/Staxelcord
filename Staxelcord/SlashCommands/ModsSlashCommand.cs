using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using AetharNet.Moonbow.Experimental.Utilities;
using AetharNet.Staxelcord.Utilities;
using Discord;
using Discord.WebSocket;
using Staxel;

namespace AetharNet.Staxelcord.SlashCommands
{
    internal static class ModsSlashCommand
    {
        public const string CommandName = "mods";
        
        public static SlashCommandProperties Build()
        {
            return new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("List all mods installed on the server")
                .Build();
        }

        public static Task Execute(SocketSlashCommand command)
        {
            var mods = GameContext.ModdingController.AccessField<IDictionary>("_mods");
            
            var modList = (from mod in mods.Values.Cast<object>()
                let workshopId = mod.AccessField<string>("WorkShopID")
                let modName = mod.AccessField<string>("ModName")
                select workshopId == "NA"
                    ? modName
                    : $"[{modName}](https://steamcommunity.com/sharedfiles/filedetails/?id={workshopId})").ToList();
            
            var splitList = DiscordUtilities.SplitMessage(string.Join("\n", modList)).Select(modLinks => new EmbedFieldBuilder {Name = "Mods Installed", Value = modLinks, IsInline = false}).ToList();
            
            var modListEmbed = new EmbedBuilder
            {
                Title = $"Currently Installed Mods",
                Color = new Color(0x7ed271),
                Fields = splitList
            }.Build();
            
            return command.RespondAsync($"There are currently **{modList.Count}** mods installed.", embed: modListEmbed);
        }
    }
}