using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shared;
using System.Threading;
using Memory;
using System.Runtime.InteropServices; // User32.dll (and dll import)
using GodOfWar;
using GodOfWar.EventListeners;

namespace ETS2_DualSenseAT_Mod
{
    public partial class Form1 : Form
    {

        static bool TouchRGBAnim = true;

        private Mem meme = new Mem();
        static bool Connect()
        {
            try
            {
                Session.client = new UdpClient();
                var portNumber = File.ReadAllText(@"C:\Temp\DualSenseX\DualSenseX_PortNumber.txt");
                Session.endPoint = new IPEndPoint(Triggers.localhost, Convert.ToInt32(portNumber));
                Session.Server_Initialized = true;
               // Server_Initialized = true;
                return true;
            }catch(Exception ex)
            {
                Session.Server_Initialized = false;
               // Server_Initialized = false;
                return false;
            }
        }

        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInf);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        private bool SearchPixel(string hexcode)
        {
            // Take an image from the screen
            // Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height); // Create an empty bitmap with the size of the current screen 

            Bitmap bitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height); // Create an empty bitmap with the size of all connected screen 

            Graphics graphics = Graphics.FromImage(bitmap as Image); // Create a new graphics objects that can capture the screen

            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size); // Screenshot moment → screen content to graphics object

            Color desiredPixelColor = ColorTranslator.FromHtml(hexcode);

            // Go one to the right and then check from top to bottom every pixel (next round -> go one to right and go down again)
            for (int x = 0; x < SystemInformation.VirtualScreen.Width; x++)
            {
                for (int y = 0; y < SystemInformation.VirtualScreen.Height; y++)
                {
                    // Get the current pixels color
                    Color currentPixelColor = bitmap.GetPixel(x, y);

                    // Finally compare the pixels hex color and the desired hex color (if they match we found a pixel)
                    if (desiredPixelColor == currentPixelColor)
                    {
                        //Console.WriteLine("Found Pixel - Now set mouse cursor");
                        //DoubleClickAtPosition(x, y);
                        return true;
                    }

                }
            }

             //Console.WriteLine("Did not find pixel");
            return false;
        }

        static void Send(Packet data)
        {
            if (!Session.Server_Initialized)
                return;
            var RequestData = Encoding.ASCII.GetBytes(Triggers.PacketToJson(data));
            Session.client.Send(RequestData, RequestData.Length, Session.endPoint);
        }

        public Form1()
        {
            InitializeComponent();

            statusLbl.Text = "Status: Ready!";
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //PixelSearchWorker.RunWorkerAsync();
            //EventsWorker.RunWorkerAsync();

            //Process[] pname = Process.GetProcessesByName("GoW");
            //if (pname.Length == 0)
            //{
            //    MessageBox.Show("God Of War isn't running, please open game first!", "DualSense AT Mod");
            //    Application.Exit();
            //}

            if (!Connect())
            {
                MessageBox.Show("Failed to connect to the DSX UDP Server ("+ Triggers.localhost, Convert.ToInt32(File.ReadAllText(@"C:\Temp\DualSenseX\DualSenseX_PortNumber.txt")) + ")");
                Application.Exit();
            }

            int PID = meme.GetProcIdFromName("GoW");

            if (PID > 0)
            {
                timer1.Enabled = true;

                //gameStaticTriggerValues();
                meme.OpenProcess(PID);
            }
            else
            {
                MessageBox.Show("God Of War isn't running, please open game first!", "DualSense AT Mod");
                Application.Exit();
            }

            //meme.WriteMemory("GoW.exe+023A7BB0,388,220,120,170,18,0,18", "float", "100");

            
        }

        static int iStep = 0;
        static int iMaxSteps = 0;
        private void InitializationEffect()
        {

            if (iMaxSteps < 5){
                Packet p = new Packet();

                int controllerIndex = 0;
                p.instructions = new Instruction[4];

                if (iStep == 0)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 27, 27, 27 };
                    
                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, true, false, false, false, false };
                    
                    iStep = 1;
                }
                else if (iStep == 1)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 169, 169, 169 };
                    
                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, true, false, false, false };
                    
                    iStep = 2;
                }
                else if (iStep == 2)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 242, 243, 244 };
                    
                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, false, true, false, false };
                    
                    iStep = 3;
                }
                else if (iStep == 3)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 146, 41, 41 };
                   
                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, false, false, true, false };
                    

                    iStep = 4;
                }
                else if (iStep == 4)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 102, 0, 0 };
                    
                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, false, false, false, true };
                    

                    iStep = 0;
                    iMaxSteps += +1;
                }

                

                Send(p);
            }
            else
            {
                Packet p = new Packet();

                int controllerIndex = 0;
                p.instructions = new Instruction[4];

                p.instructions[0].type = InstructionType.RGBUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, 199, 24, 24 };

                // PLAYER LED 1-5 true/false state
                p.instructions[1].type = InstructionType.PlayerLED;
                p.instructions[1].parameters = new object[] { controllerIndex, false, false, false, false, false };

                Send(p);

                timer1.Enabled = false;
                everyTick.Enabled = true;
                //gameStaticTriggerValues();
            }

        }

        private void gameStaticTriggerValues()
        {

            Packet p = new Packet();

            int controllerIndex = 0;
            p.instructions = new Instruction[4];

            p.instructions[0].type = InstructionType.TriggerUpdate;
            p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Right, TriggerMode.SemiAutomaticGun, 2, 7, 8 };

            p.instructions[1].type = InstructionType.TriggerUpdate;
            p.instructions[1].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Bow, 0, 1, 8, 8};
            
            p.instructions[2].type = InstructionType.RGBUpdate;
            p.instructions[2].parameters = new object[] { controllerIndex, 102, 0, 0 };

            Send(p);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Packet p = new Packet();

            int controllerIndex = 0;

            p.instructions = new Instruction[4];

            p.instructions[0].type = InstructionType.TriggerUpdate;
            p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Right, TriggerMode.Normal };


            p.instructions[1].type = InstructionType.TriggerUpdate;
            p.instructions[1].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Normal };


            p.instructions[2].type = InstructionType.RGBUpdate;
            p.instructions[2].parameters = new object[] { controllerIndex, 66, 135, 245 };

            Send(p);
            statusLbl.Text = "Status: Closing";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            InitializationEffect();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            TouchRGBAnim = checkBox1.Checked;
        }

        static int LED_Step = 0;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Packet p = new Packet();

            int controllerIndex = 0;

            p.instructions = new Instruction[4];

            if (LED_Step == 0)
            {
                p.instructions[0].type = InstructionType.RGBUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, 255, 0, 0 };

                LED_Step = 1;
            }
            else
            {
                if (LED_Step == 1)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 166, 0, 0 };

                    LED_Step = 0;
                }
            }

            Thread.Sleep(950);
            Send(p);
        }

        private void LowHealth()
        {
            if (iMaxSteps < 5)
            {
                Packet p = new Packet();

                int controllerIndex = 0;
                p.instructions = new Instruction[4];

                if (iStep == 0)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 252, 7, 3 };

                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, true, false, false, false, false };

                    iStep = 1;
                }
                else if (iStep == 1)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 166, 58, 58 };

                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, true, false, false, false };

                    iStep = 2;
                }
                else if (iStep == 2)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 150, 101, 101 };

                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, false, true, false, false };

                    iStep = 3;
                }
                else if (iStep == 3)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 107, 0, 0 };

                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, false, false, true, false };


                    iStep = 4;
                }
                else if (iStep == 4)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 38, 250, 5 };

                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, false, false, false, true };


                    iStep = 0;
                    iMaxSteps += +1;
                }



                Send(p);
            }
            else
            {
                Packet p = new Packet();

                int controllerIndex = 0;
                p.instructions = new Instruction[4];

                p.instructions[0].type = InstructionType.RGBUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, 199, 24, 24 };

                // PLAYER LED 1-5 true/false state
                p.instructions[1].type = InstructionType.PlayerLED;
                p.instructions[1].parameters = new object[] { controllerIndex, false, false, false, false, false };

                Send(p);
            }
        }

        private void memory_Tick(object sender, EventArgs e)
        {
            
        }

        private void dualsense_triggers_DoWork(object sender, DoWorkEventArgs e)
        {
            LowHealth();
        }

        private void ShowLights_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void everyTick_Tick(object sender, EventArgs e)
        {
            if (!seconThread.IsBusy)
                seconThread.RunWorkerAsync();

            if (!PixelSearchWorker.IsBusy)
                PixelSearchWorker.RunWorkerAsync();

            if (!EventsWorker.IsBusy)
                EventsWorker.RunWorkerAsync();
        }

        private void seconThread_DoWork(object sender, DoWorkEventArgs e)
        {
            float health = meme.ReadFloat("GoW.exe+011AC280,9A0,30,40,8,388");

            if (health < 20)
            {
                LowHealth();
            }
            else
            {

            }
        }

        private void PixelSearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Console.Write("Searching!");
            PixelSearchWorker.RunWorkerAsync();
        }

        private void PixelSearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
           // Console.Write("Searching!");
            if (SearchPixel("#AFA17D") && SearchPixel("#1C1C1C"))
            {
                Session.is_Paused = true;
                Console.WriteLine("MENU PAUSE");

            }
            else
            {
                Session.is_Paused = false;
                Console.WriteLine("MENU IS NOT IN PAUSE");
            }
            //SearchPixel("#E6CE92");
           // SearchPixel("#E8D18E");
            //SearchPixel("#CAA35C");
        }

        private void EventsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            EventsWorker.RunWorkerAsync();
        }

        private void EventsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //If game is paused
            if (Session.is_Paused)
            {
                //Console.WriteLine("Call menuPaused();");
                MainEvents.menuPaused();

            }
            //If Game is not Paused.
            else
            {
                //Console.WriteLine("Call AxeHold();");
                MainEvents.AxeHold();
            }


        }
    }
}
