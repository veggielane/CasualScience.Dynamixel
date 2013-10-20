using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CasualScience.Dynamixel.Packets;

namespace CasualScience.Dynamixel
{
 
    public class DynamixelBus : IDisposable
    {
        private readonly DynamixelPort _port;

        private readonly Subject<IPacket> _subject;
        public IObservable<IPacket> Packets { get; private set; }

        //private readonly Dictionary<byte, DynamixelDevice> _dynamixelDevices = new Dictionary<byte, DynamixelDevice>();
        //public ReadOnlyDictionary<byte, DynamixelDevice> Devices
        //{
        //    get { return new ReadOnlyDictionary<byte, DynamixelDevice>(_dynamixelDevices); }
        //}

        public DynamixelBus(string portName, int baudRate)
        {
            _port = new DynamixelPort("COM3", 1000000);
            _subject = new Subject<IPacket>();
            Packets = _subject.AsObservable();

            _port.StatusPacketReceived += Add;

            Packets.OfType<InstructionPacket>().Subscribe(p => _port.Write(p));
        }

        public void Discover()
        {
            Add(new InstructionPacket(0xFE, Instruction.Ping)
            {
                Parameters = new byte[] {}
            });

        }

        public void Add(IPacket message)
        {
            _subject.OnNext(message);
        }

        public void Dispose()
        {
            _port.Dispose();
        }

        public int this[byte index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set { throw new NotImplementedException(); }
        }
    }

    public class DynamixelDevice
    {
        
    }
}
