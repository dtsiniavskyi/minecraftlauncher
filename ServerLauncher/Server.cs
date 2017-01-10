using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MinecraftServerLauncher
{
    public class Server
    {
        public string Name { get; set; }
        public string Folder { get; set; }

        public string Starter
        {
            get
            {
                return Path.Combine(Folder, "start.bat");
            }
        }
        public string WorldsFolder
        {
            get
            {
                return Path.Combine(Folder, "worlds");
            }
        }

        public IEnumerable<World> Worlds { get; set; }

        public void Start(World world)
        {
            // Modify properties

            string props;
            using (StreamReader sr = new StreamReader(Path.Combine(Folder, "server.properties")))
            {
                props = sr.ReadToEnd();
            }

            var lines = props.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            var newSettings = string.Empty;
            foreach (var line in lines)
            {
                var newLine = line;
                if (line.StartsWith("server-port"))
                {
                    newLine = line.Replace(line, "server-port=" + world.Port);
                }

                if (line.StartsWith("level-name"))
                {
                    newLine = line.Replace(line, "level-name=worlds/" + world.Name);
                }

                newSettings = newSettings += (newLine + "\r\n");
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(Folder, "server.properties")))
            {
                sw.Write(newSettings);
            }


            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WorkingDirectory = Folder;
            p.StartInfo.Arguments = @"/C " + Starter;
            //p.StartInfo
            p.Start();
            p.WaitForExit();
        }
    }
}
