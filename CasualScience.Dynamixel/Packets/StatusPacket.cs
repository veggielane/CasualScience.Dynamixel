using System.Collections.Generic;

namespace CasualScience.Dynamixel.Packets
{
    public class StatusPacket:BasePacket
    {
        private readonly IList<byte> _data;

        public StatusPacket(IList<byte> data)
        {
            _data = data;
        }

        public override IEnumerable<byte> BuildPacket()
        {
            return _data;
        }

        public override byte CheckSum()
        {
            return 0;
        }
        public override string ToString()
        {
            return "RX < " + base.ToString();
        }
    }
}