using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GenericXMLSerializer;

namespace MinecraftServerLauncher
{
    public partial class Main : Form
    {
        private Settings stings;
        private IEnumerable<Server> servers;
        private IEnumerable<World> worlds;
        
        public Main()
        {
            InitializeComponent();
            try
            {
                stings = XMLHandler<Settings>.Read(Constants.SettingsFileName);
            }
            catch 
            {
                
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new SettingsForm(this);
            s.ShowDialog();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                LoadServers();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can not load servers. Change server folder location in settings and try again.", "Error");
            }
            
        }

        public void LoadServers()
        {
            // load servers
            servers = new DirectoryInfo(stings.ServersFolder).GetDirectories().Select(s => new Server
            {
                Folder = s.FullName,
                Name = s.Name,
                Worlds = LoadWorlds(Path.Combine(s.FullName, "worlds"))
            }).ToList();

            listBox1.Items.Clear();
            foreach (var server in servers)
            {
                listBox1.Items.Add(server.Name);
                //server.Worlds = LoadWorlds(server);
            }
            listBox1.SelectedIndex = string.IsNullOrEmpty(stings.SelectedServer) ? 0 : listBox1.Items.IndexOf(stings.SelectedServer);
            
            listBox2.Items.Clear();
            
            var selectedServer = string.IsNullOrEmpty(stings.SelectedServer) 
                ? servers.FirstOrDefault()
                : servers.FirstOrDefault(s => s.Name.Equals(stings.SelectedServer));

            if (selectedServer.Worlds.Any())
            {
                foreach (var serverWorld in selectedServer.Worlds)
                {
                    listBox2.Items.Add(serverWorld.Name);
                }
                listBox2.SelectedIndex = string.IsNullOrEmpty(stings.SelectedWorld) ? 0 : listBox2.Items.IndexOf(stings.SelectedWorld);
            }
        }

        public IEnumerable<World> LoadWorlds(string worldsFolder)
        {
            // load worlds
            try
            {
                var serverWorlds =
                    new DirectoryInfo(worldsFolder)
                        .GetDirectories().Select(d => new World
                        {
                            Name = d.Name,
                            Port = d.Name.Split('_')[1]
                        }).ToList();

                return serverWorlds;
            }
            catch
            {
                return Enumerable.Empty<World>();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedServer = listBox1.SelectedItem;
                var server = servers.FirstOrDefault(s => s.Name.Equals(selectedServer));

                var serverWorlds = new DirectoryInfo(server.WorldsFolder).GetDirectories().Select(d => new World
                {
                    Name = d.Name,
                    Port = d.Name.Split('_')[1]
                }).ToList();
                listBox2.Items.Clear();

                foreach (var serverWorld in serverWorlds)
                {
                    listBox2.Items.Add(serverWorld.Name);
                }
            }
            catch
            {
                listBox2.Items.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RunServer();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            RunClient();
            Close();
        }

        private void RunClient()
        {
            var path = Path.Combine(stings.MinecraftFolder, "Minecraft.exe");
            Process.Start(path);
        }

        private void RunServer()
        {
            var settings = stings;
            settings.SelectedServer = listBox1.SelectedItem != null ? listBox1.SelectedItem.ToString() : null;
            settings.SelectedWorld = listBox2.SelectedItem != null ? listBox2.SelectedItem.ToString() : null;

            XMLHandler<Settings>.Write(settings, Constants.SettingsFileName);
            Close();

            var selectedServer = servers.FirstOrDefault(s => s.Name.Equals(settings.SelectedServer));
            var world = selectedServer.Worlds.FirstOrDefault(w => w.Name.Equals(settings.SelectedWorld));

            selectedServer.Start(world);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RunClient();
            RunServer();
        }
    }
}
