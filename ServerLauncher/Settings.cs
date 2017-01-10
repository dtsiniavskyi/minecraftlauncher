using System.IO;

namespace MinecraftServerLauncher
{
    public class Settings
    {
        public string MinecraftFolder { get; set; }

        public string ServersFolder
        {
            get { return Path.Combine(MinecraftFolder, "servers"); }
        }

        public string SelectedServer { get; set; }
        public string SelectedWorld { get; set; }
    }
}
