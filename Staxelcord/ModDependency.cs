using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace AetharNet.Staxelcord
{
    // Credit to NimbusFox for this great utility class!
    public class ModDependency : IModHookV4
    {
        private static readonly DirectoryInfo ModDirectory = new(Path.Combine(GameContext.ContentLoader.RootDirectory, "mods"));

        static ModDependency() => VerifyDependency("Staxelcord", "Moonbow.Experimental");

        private static void VerifyDependency(string modName, string dependencyName)
        {
            if (ModDirectory.GetDirectories().Any(dir => dir.Name == dependencyName)) return;

            var message = $"You are missing the following mod: {QuoteWrap(dependencyName)}\nPlease install {QuoteWrap(dependencyName)} or uninstall {QuoteWrap(modName)}";
            
            try
            {
                MessageBox.Show(message, "Missing Mod", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch
            {
                Console.WriteLine(message);
                Console.ReadKey();
            }
            
            Environment.Exit(0);
        }

        private static string QuoteWrap(string input) => '"' + input + '"';
        
        public virtual void Dispose() {}
        public virtual void CleanupOldSession() {}
        
        public virtual void UniverseUpdateBefore(Universe universe, Timestep step) {}
        public virtual void UniverseUpdateAfter() {}

        public virtual void GameContextInitializeInit() {}
        public virtual void GameContextInitializeBefore() {}
        public virtual void GameContextInitializeAfter() {}
        public virtual void GameContextDeinitialize() {}
        public virtual void GameContextReloadBefore() {}
        public virtual void GameContextReloadAfter() {}
        
        public virtual void ClientContextInitializeInit() {}
        public virtual void ClientContextInitializeBefore() {}
        public virtual void ClientContextInitializeAfter() {}
        public virtual void ClientContextDeinitialize() {}
        public virtual void ClientContextReloadBefore() {}
        public virtual void ClientContextReloadAfter() {}
        
        public virtual bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) => true;
        public virtual bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) => true;
        public virtual bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) => true;
        
        public virtual bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) => true;
        public virtual bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) => true;
    }
}