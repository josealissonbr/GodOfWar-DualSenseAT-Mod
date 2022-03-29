using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ETS2_DualSenseAT_Mod
{
    public partial class Setup : Form
    {
        public Setup()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            if (!Directory.Exists(textBox1.Text))
            {
                MessageBox.Show("The path entered is not valid!", "DualSense AT Mod");
                return;
            }

            if (!File.Exists(textBox1.Text + "\\bin\\win_x64\\eurotrucks2.exe"))
            {
                MessageBox.Show("eurotrucks2.exe not found on " + textBox1.Text + "\\bin\\win_x64\\", "DualSense AT Mod");
                return;
            }

            if (!File.Exists(textBox1.Text + "\\bin\\win_x64\\plugins\\ets2-telemetry.dll"))
            {
                File.Copy(Application.StartupPath + "\\ets2-telemetry.dll", textBox1.Text + "\\bin\\win_x64\\plugins\\ets2-telemetry.dll");
            }

            var Settings = new IniFile(@"C:\Temp\DualSenseX\DualSenseAT\games\" + Constants.app_id + @"\settings.ini");

            Settings.Write("game_path", textBox1.Text);

            MessageBox.Show("Data saved successfully! the application will restart to apply the update.", "DualSense AT Mod");
            Application.Restart();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            button1.Enabled = false;

            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            
        }

        private void Setup_Load(object sender, EventArgs e)
        {

        }
    }
}
