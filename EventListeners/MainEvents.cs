using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GodOfWar.EventListeners
{
    internal class MainEvents
    {

        static void Send(Packet data)
        {
            //if (!Session.Server_Initialized)
                //return;
            var RequestData = Encoding.ASCII.GetBytes(Triggers.PacketToJson(data));
            Session.client.Send(RequestData, RequestData.Length, Session.endPoint);
        }

        public static void menuPaused()
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
        }

        public static void AxeHold()
        {

            Packet p = new Packet();

            int controllerIndex = 0;
            p.instructions = new Instruction[4];

            p.instructions[0].type = InstructionType.TriggerUpdate;
            p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Right, TriggerMode.Hard/*, 2, 7, 8*/ };

            p.instructions[1].type = InstructionType.TriggerUpdate;
            p.instructions[1].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Bow, 0, 1, 8, 8 };

            //p.instructions[2].type = InstructionType.RGBUpdate;
            //p.instructions[2].parameters = new object[] { controllerIndex, 102, 0, 0 };

            Send(p);

        }


    }
}
