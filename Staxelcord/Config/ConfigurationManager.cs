using System;
using AetharNet.Moonbow.Experimental.Utilities;
using AetharNet.Staxelcord.Hooks;
using Plukit.Base;

namespace AetharNet.Staxelcord.Config
{
    internal static class ConfigurationManager
    {
        private const string ModName = "Staxelcord";

        private const string ConfigFileMainName = "config.json";
        private const string ConfigFileExampleName = "config.example.json";

        private const string ConfigKeyVersion = "__version__";
        private const string ConfigKeyEnabled = "enabled";
        private const string ConfigKeyToken = "token";
        private const string ConfigKeyChannelId = "channelID";

        private static WorldModDirectoryManager _directoryManager;
        
        public static bool TryGetConfig(out Configuration configuration)
        {
            _directoryManager ??= new WorldModDirectoryManager(ModName);

            if (_directoryManager.FileExists(ConfigFileMainName))
            {
                try
                {
                    var configBlob = _directoryManager.ReadFileAsBlob(ConfigFileMainName);
                    
                    var enabled = configBlob.GetBool(ConfigKeyEnabled, false);
                    var token = configBlob.GetString(ConfigKeyToken);
                    var channelId = configBlob.GetEntryKind(ConfigKeyChannelId) == BlobEntryKind.String
                        ? ulong.Parse(configBlob.GetString(ConfigKeyChannelId))
                        : (ulong) configBlob.GetLong(ConfigKeyChannelId);

                    Blob.Deallocate(ref configBlob);
                    
                    configuration = new Configuration
                    {
                        Version = 1,
                        Enabled = enabled,
                        Token = token,
                        ChannelId = channelId
                    };
                    return true;
                }
                catch (Exception ex)
                {
                    var exceptionMessage = ex.Message
                        .Replace("json", "JSON")
                        .Replace("None", "(Root Object)");
                    
                    AdminLogHook.AddLog($"Encountered error while retrieving configuration file: {exceptionMessage}");
                }
            }
            else if (!_directoryManager.FileExists(ConfigFileExampleName))
            {
                var exampleConfig = BlobAllocator.Blob(true);
                
                exampleConfig.SetLong(ConfigKeyVersion, 1);
                exampleConfig.SetBool(ConfigKeyEnabled, true);
                exampleConfig.SetString(ConfigKeyToken, "TOKEN.GOES.HERE");
                exampleConfig.SetLong(ConfigKeyChannelId, 0);
                
                _directoryManager.WriteFileFromBlob(ConfigFileExampleName, exampleConfig);
                
                Blob.Deallocate(ref exampleConfig);
            }

            configuration = new Configuration();
            return false;
        }
    }
}