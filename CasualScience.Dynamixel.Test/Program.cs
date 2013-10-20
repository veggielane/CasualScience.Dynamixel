using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CasualScience.Dynamixel.Packets;

namespace CasualScience.Dynamixel.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = new DynamixelBus("COM3", 1000000))
            {
                bus.Packets.OfType<StatusPacket>().Subscribe(p => Console.WriteLine(p.ToString()));
                bus.Discover();

                //bus[3].
                

                while (true)
                {
                    Console.ReadLine();
                    bus.Add(new InstructionPacket(1, Instruction.WriteData)
                    {
                        Parameters = new byte[] { 0x19, 0x01 }
                    });
                    Console.ReadLine();
                    bus.Add(new InstructionPacket(1, Instruction.WriteData)
                    {
                        Parameters = new byte[] { 0x19, 0x00 }
                    });
                }
            }


            //using (var d = new DynamixelPort("COM3", 1000000))
            //{
            //    d.Write(new InstructionPacket(1, Instruction.ReadData)
            //    {
            //        Parameters = new byte[] { 0x2b, 0x01 }
            //    });
            //    while (true)
            //    {
            //        Console.ReadLine();
            //        d.Write(new InstructionPacket(1, Instruction.WriteData)
            //        {
            //            Parameters = new byte[] { 0x19, 0x01 }
            //        });
            //        Console.ReadLine();
            //        d.Write(new InstructionPacket(1, Instruction.WriteData)
            //        {
            //            Parameters = new byte[] { 0x19, 0x00 }
            //        });
            //    }
            //}
        }
    }
}
