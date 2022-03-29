using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ETS2_DualSenseAT_Mod
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!Directory.Exists(@"C:\Temp\DualSenseX\DualSenseAT\games\"+Constants.app_id))
                Directory.CreateDirectory(@"C:\Temp\DualSenseX\DualSenseAT\games\" + Constants.app_id);

            var Settings = new IniFile(@"C:\Temp\DualSenseX\DualSenseAT\games\" + Constants.app_id+@"\settings.ini");

            //if (!Directory.Exists(Settings.Read("game_path")))
            //    Application.Run(new Setup());
            //else
            //    Application.Run(new Form1());

            Application.Run(new Form1());
        }
    }
}
