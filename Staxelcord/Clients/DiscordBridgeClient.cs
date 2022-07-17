using AetharNet.Moonbow.Experimental.Utilities;
using AetharNet.Staxelcord.Config;
using AetharNet.Staxelcord.Utilities;

namespace AetharNet.Staxelcord.Clients
{
    internal class DiscordBridgeClient
    {
        private static readonly string StaxelChatPrefix = ChatFormat.Format("[Discord]", TextStyling.Bold, "#5865F2");

        private Configuration _configuration;

        public DiscordBridgeClient(Configuration configuration) => _configuration = configuration;

        public void Initialize()
        {
            // Connect to Discord, validate and retrieve channel, and assign event handlers
        }
        
        public bool IsConnected()
        {
            // Return client's connection status
            
            return false;
        }

        public void BridgeStaxelMessage(string playerName, string message)
        {
            var cleanedName = DiscordUtilities.EscapeContent(playerName);
            var cleanedMessage = DiscordUtilities.EscapeContent(message);
            
            // Send message to bridged channel
        }
    }
}