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

namespace ETS2_DualSenseAT_Mod
{
    public partial class Form1 : Form
    {

        static UdpClient client;
        static IPEndPoint endPoint;

        static bool Server_Initialized = false; 
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
            Process[] pname = Process.GetProcessesByName("bio4");
            if (pname.Length == 0)
            {
                MessageBox.Show("God Of War isn't running, please open game first!", "DualSense AT Mod");
                Application.Exit();
            }

            if (!Connect())
            {
                MessageBox.Show("Failed to connect to the DSX UDP Server ("+ Triggers.localhost, Convert.ToInt32(File.ReadAllText(@"C:\Temp\DualSenseX\DualSenseX_PortNumber.txt")) + ")");
                Application.Exit();
            }


            timer1.Enabled = true;
            
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
                    p.instructions[0].parameters = new object[] { controllerIndex, 237, 61, 7 };
                    
                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, true, false, false, false, false };
                    
                    iStep = 1;
                }
                else if (iStep == 1)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 252, 0, 0 };
                    
                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, true, false, false, false };
                    
                    iStep = 2;
                }
                else if (iStep == 2)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 148, 22, 0 };
                    
                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, false, true, false, false };
                    
                    iStep = 3;
                }
                else if (iStep == 3)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 237, 61, 7 };
                   
                    // PLAYER LED 1-5 true/false state
                    p.instructions[1].type = InstructionType.PlayerLED;
                    p.instructions[1].parameters = new object[] { controllerIndex, false, false, false, true, false };
                    

                    iStep = 4;
                }
                else if (iStep == 4)
                {
                    p.instructions[0].type = InstructionType.RGBUpdate;
                    p.instructions[0].parameters = new object[] { controllerIndex, 148, 22, 0 };
                    
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
                p.instructions[0].parameters = new object[] { controllerIndex, 119, 3, 252 };

                // PLAYER LED 1-5 true/false state
                p.instructions[1].type = InstructionType.PlayerLED;
                p.instructions[1].parameters = new object[] { controllerIndex, true, false, false, false, false };

                Send(p);

                timer1.Enabled = false;
                gameStaticTriggerValues();
            }

        }

        private void gameStaticTriggerValues()
        {

            Packet p = new Packet();

            int controllerIndex = 0;
            p.instructions = new Instruction[4];

            p.instructions[0].type = InstructionType.TriggerUpdate;
            p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Right, TriggerMode.CustomTriggerValue, CustomTriggerValueMode.VibratePulseAB, 61, 255, 255, 5, 67, 119, 0 };

            p.instructions[1].type = InstructionType.TriggerUpdate;
            p.instructions[1].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Bow, 0, 1, 8, 8};

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
    }
}
