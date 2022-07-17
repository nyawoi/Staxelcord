using AetharNet.Moonbow.Experimental.Interfaces;
using AetharNet.Moonbow.Experimental.Templates;
using AetharNet.Staxelcord.Clients;
using AetharNet.Staxelcord.Config;
using Staxel.Commands;
using Staxel.Logic;

namespace AetharNet.Staxelcord.Hooks
{
    internal class StaxelcordHook : ServerMessagingHookTemplate, IServerMessagingHook
    {
        private static readonly DiscordBridgeClient Discord;

        static StaxelcordHook()
        {
            if (!ConfigurationManager.TryGetConfig(out var configuration) || !configuration.Enabled) return;
            
            Discord = new DiscordBridgeClient(configuration);
            Discord.Initialize();
        }
        
        public override void OnPlayerMessageReceived(EntityId entityId, string message, ICommandsApi api)
        {
            if (Discord == null || !Discord.IsConnected()) return;
            
            if (!api.TryGetEntity(entityId, out var playerEntity)) return;
            
            Discord.BridgeStaxelMessage(playerEntity.PlayerEntityLogic.DisplayName(), message);
        }
    }
}