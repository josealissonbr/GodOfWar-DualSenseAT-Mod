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

namespace ETS2_DualSenseAT_Mod
{
    public partial class Form1 : Form
    {

        static UdpClient client;
        static IPEndPoint endPoint;

        static bool Server_Initialized = false; 

        static bool TouchRGBAnim = true;

        private Mem meme = new Mem();
        static bool Connect()
        {
            try
            {
                client = new UdpClient();
                var portNumber = File.ReadAllText(@"C:\Temp\DualSenseX\DualSenseX_PortNumber.txt");
                endPoint = new IPEndPoint(Triggers.localhost, Convert.ToInt32(portNumber));
                Server_Initialized = true;
                return true;
            }catch(Exception ex)
            {
                Server_Initialized = false;
                return false;
            }
        }

        static void Send(Packet data)
        {
            if (!Server_Initialized)
                return;
            var RequestData = Encoding.ASCII.GetBytes(Triggers.PacketToJson(data));
            client.Send(RequestData, RequestData.Length, endPoint);
        }

        public Form1()
        {
            InitializeComponent();

            statusLbl.Text = "Status: Ready!";
        }


        private void Form1_Load(object sender, EventArgs e)
        {
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

            int PID = meme.GetProcIdFromName("gow");

            if (PID > 0)
            {
                timer1.Enabled = true;

                //gameStaticTriggerValues();
                //meme.OpenProcess(PID);
            }
            else
            {
                MessageBox.Show("God Of War isn't running, please open game first!", "DualSense AT Mod");
                Application.Exit();
            }

            //meme.WriteMemory("GoW.exe+023A7BB0,388,220,120,170,18,0,18", "float", "100");

            // int health = memLib.ReadInt("GoW.exe+01278340,388,8");
            //memLib.WriteMemory("GoW.exe+01278340,388,8", "int", "50");
            //float health = meme.ReadFloat("GoW.exe+023A7BB0,388,220,120,170,18,0,18");
            //label1.Text = health.ToString();

            //timer1.Enabled = true;

            //memory.Enabled = true;

            // gameStaticTriggerValues();
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
                TouchAnim.Enabled = true;
                gameStaticTriggerValues();
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

        private void TouchAnim_Tick(object sender, EventArgs e)
        {
            
            if (!TouchRGBAnim)
                return;

            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
            
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

        private void memory_Tick(object sender, EventArgs e)
        {
           // memLib.WriteMemory("GoW.exe+01278340,388,8", "float", "50");
            //memLib.WriteMemory("GoW.exe+01278340,5B8,C8,8", "float", "2");
            //float health = memLib.ReadInt("GoW.exe+01278340,5B8,C8,8");
            //label1.Text = health.ToString();
        }
    }
}
