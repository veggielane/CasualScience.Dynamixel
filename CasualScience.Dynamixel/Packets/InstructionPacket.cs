using System.Collections.Generic;
using System.Linq;

namespace CasualScience.Dynamixel.Packets
{
    public class InstructionPacket : BasePacket
    {
        public byte Id { get; private set; }

        public int Length
        {
            get { return Parameters != null && Parameters.Length > 0 ? 2 + Parameters.Length : 2; }
        }

        public Instruction Instruction;

        public byte[] Parameters;

        public InstructionPacket(byte id, Instruction instruction)
        {
            Id = id;
            Instruction = instruction;

        }


        public override IEnumerable<byte> BuildPacket()
        {
            yield return 0xff;
            yield return 0xff;
            yield return Id;
            yield return (byte) Length;
            yield return (byte) Instruction;
            if ((Parameters != null) && (Parameters.Length > 0))
            {
                foreach (var parameter in Parameters)
                {
                    yield return parameter;
                }
            }
            yield return CheckSum();
        }

        public override byte CheckSum()
        {
            var sum = (byte)(Id + (byte) Length + (byte) Instruction);
            if ((Parameters != null) && (Parameters.Length > 0))
            {
                sum = Parameters.Aggregate(sum, (current, parameter) => (byte) (current + parameter));
            }
            return (byte) ~(sum & 0xFF);
        }
        public override string ToString()
        {
            return "TX > " + base.ToString();
        }
    }

    public enum Instruction : byte
    {
        Ping = 0x01,
        ReadData = 0x02,
        WriteData = 0x03,
        RegWrite = 0x04,
        Action = 0x05,
        SyncWrite = 0x83,

        //USB2AX Advanced Instructions
        Reset = 0x06,
        BootLoader = 0x08,
        SyncRead = 0x84,
    }
}