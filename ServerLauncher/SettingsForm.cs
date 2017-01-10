using System;
using System.Windows.Forms;
using GenericXMLSerializer;

namespace MinecraftServerLauncher
{
    public partial class SettingsForm : Form
    {
        private Main _mainForm;

        public SettingsForm(Main mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var message = @"To keep program work properly follow the next rules:
1. Kepp all servers in the same directory.
2. Keep all server worlds in 'worlds' folder.
3. Create starter bat for each server and call it 'start'.
4. Name your world folder by the following pattern 'world-name_PORT'
5. Name main game launcher Minecraft.exe";
            var title = @"Information";

            title.Replace(@"\", string.Empty);

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var settings = new Settings
            {
                MinecraftFolder = textBox1.Text
            };

            XMLHandler<Settings>.Write(settings, Constants.SettingsFileName);
            this.Close();
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _mainForm.LoadServers();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            var currentSettings = XMLHandler<Settings>.Read(Constants.SettingsFileName);
            textBox1.Text = currentSettings.MinecraftFolder;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
