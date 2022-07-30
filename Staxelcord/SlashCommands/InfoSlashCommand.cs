using System;
using System.Linq;
using System.Threading.Tasks;
using AetharNet.Moonbow.Experimental.Utilities;
using Discord;
using Discord.WebSocket;

namespace AetharNet.Staxelcord.SlashCommands
{
    public class InfoSlashCommand
    {
        public const string CommandName = "info";

        public static SlashCommandProperties Build()
        {
            return new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("View server details")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("details")
                    .WithDescription("Choose which details to view")
                    .WithRequired(true)
                    .AddChoice("all", "all")
                    .AddChoice("time", "time")
                    .AddChoice("versions", "versions")
                    .AddChoice("generation", "generation")
                    .WithType(ApplicationCommandOptionType.String))
                .Build();
        }

        public static Task Execute(SocketSlashCommand command)
        {
            var worldGenInfo = GameUtilities.ServerMainLoop.WorldManager.GetWorldGenInfo();
            var unixTimestamp = new DateTimeOffset(worldGenInfo.CreationDate).ToUnixTimeSeconds();
            
            var serverInfoEmbed = new EmbedBuilder
            {
                Title = "Server Information",
                Description = $"World created @ <t:{unixTimestamp}:t>",
                Color = new Color(0x7ed271)
            };

            var choice = command.Data.Options.First().Value as string;
            
            if (choice is "all" or "time")
            {
                var dayNightCycle = GameUtilities.Universe.DayNightCycle();
                var day = dayNightCycle.Day + 1;
                var phase = dayNightCycle.Phase;
                var state = dayNightCycle.GamePaused() ? "paused" : "active";
                
                serverInfoEmbed.AddField(new EmbedFieldBuilder
                {
                    Name = "Time",
                    Value = $"**Day:** {day}\n**Phase:** {phase:3}\n**State:** {state}",
                    IsInline = true
                });
            }
            
            if (choice is "all" or "versions")
            {
                var staxelVersion = worldGenInfo.StaxelVersion;
                var worldGeneratorVersion = worldGenInfo.WorldGenVersion;
                var formatVersion = worldGenInfo.FormatVersion;

                serverInfoEmbed.AddField(new EmbedFieldBuilder
                {
                    Name = "Versions",
                    Value = $"**Staxel:** {staxelVersion.Replace("Staxel ", "v")}\n**World Generator:** {worldGeneratorVersion}\n**World Config Mapping Format:** v{formatVersion}",
                    IsInline = true
                });
            }
            
            if (choice is "all" or "generation")
            {
                var worldSeed = worldGenInfo.WorldSeed;
                var worldSize = worldGenInfo.WorldSize;
                var farmCount = worldGenInfo.FarmCount;

                serverInfoEmbed.AddField(new EmbedFieldBuilder
                {
                    Name = "Generation",
                    Value = $"**World Seed:** `{worldSeed}`\n**World Size:** {worldSize}\n**Farm Count:** {farmCount}",
                    IsInline = true
                });
            }

            return command.RespondAsync(embed: serverInfoEmbed.Build());
        }
    }
}