using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AetharNet.Staxelcord.Utilities
{
    internal static class DiscordUtilities
    {
        private static Regex _escapePattern;
        
        public static string EscapeContent(string content)
        {
            _escapePattern ??= new Regex(@"[_~*`><\\|]", RegexOptions.Compiled);

            return _escapePattern.Replace(content, "\\$0");
        }
        
        public static IEnumerable<string> SplitMessage(string message, int maxLength = 1024)
        {
            var parts = new List<string>();
            
            if (message.Length < maxLength)
            {
                parts.Add(message);
                
                return parts;
            }

            do
            {
                var index = message.LastIndexOf('\n', maxLength);
                
                if (index < 1) break;
                
                parts.Add(message.Substring(0, index));
                message = message.Substring(index);
            } while (message.Length > maxLength);
            
            parts.Add(message);

            return parts;
        }
        
        public static string GetOnlineMessage(int onlineCount)
        {
            return onlineCount switch
            {
                0 => "There are **no** players currently online.",
                1 => "There is currently **one** player online.",
                _ => $"There are currently **{onlineCount}** players online."
            };
        }
    }
}