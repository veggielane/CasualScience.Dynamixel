using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualScience.Dynamixel.Test
{
    class Program
    {
        static void Main(string[] args)
        {



            using (var d = new DynamixelPort("COM3", 1000000))
            {
                d.Write(new InstructionPacket(1, Instruction.ReadData)
                {
                    Parameters = new byte[] { 0x2b, 0x01 }
                });
                while (true)
                {
                    Console.ReadLine();
                    d.Write(new InstructionPacket(1, Instruction.WriteData)
                    {
                        Parameters = new byte[] { 0x19, 0x01 }
                    });
                    Console.ReadLine();
                    d.Write(new InstructionPacket(1, Instruction.WriteData)
                    {
                        Parameters = new byte[] { 0x19, 0x00 }
                    });
                    
                }


            }
        }
    }
}
