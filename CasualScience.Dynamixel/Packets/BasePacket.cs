using System;
using System.Collections.Generic;
using System.Linq;

namespace CasualScience.Dynamixel.Packets
{
    public abstract class BasePacket:IPacket
    {
        public abstract IEnumerable<byte> BuildPacket();
        public abstract byte CheckSum();

        public byte[] ToByteArray()
        {
            return BuildPacket().ToArray();
        }

        public override string ToString()
        {
            return String.Join(" ", BuildPacket().Select(b => String.Format("0x{0:x2}", b)));
        }
    }
}