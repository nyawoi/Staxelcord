using System;
using System.Collections.Generic;
using AetharNet.Moonbow.Experimental.Interfaces;
using AetharNet.Moonbow.Experimental.Templates;
using AetharNet.Moonbow.Experimental.Utilities;
using Plukit.Base;
using Staxel;
using Staxel.Logic;

namespace AetharNet.Staxelcord.Hooks
{
    internal class AdminLogHook : ServerConnectionHookTemplate, IServerConnectionHook
    {
        private static readonly List<string> ChatLogs = new();
        private const string MessagePrefix = "[Staxelcord] ";
        private const string MessageColor = "#F1CF77";
        
        private static bool IsAdmin(Entity playerEntity)
        {
            return ServerContext.RightsManager.HasRight(
                playerEntity.PlayerEntityLogic.GetUsername(),
                playerEntity.PlayerEntityLogic.Uid(),
                "admin");
        }
        
        public override void OnPlayerConnect(Entity playerEntity)
        {
            if (playerEntity.PlayerEntityLogic == null || !IsAdmin(playerEntity)) return;

            foreach (var message in ChatLogs)
            {
                ServerMessaging.MessagePlayerPlainText(playerEntity, ChatFormat.Color(MessagePrefix + message, MessageColor));
            }
        }

        public static void AddLog(string message)
        {
            ChatLogs.Add(message);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Logger.WriteLine(MessagePrefix + message);
            Console.ResetColor();
        }
    }
}