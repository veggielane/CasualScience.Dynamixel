using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualScience.Dynamixel
{
    public class DynamixelPort:IDisposable
    {
        public delegate void StatusPacketHandler(StatusPacket packet);
        public event StatusPacketHandler StatusPacketReceived;
        private readonly SerialPort _port;
        public DynamixelPort(string portName, int baudRate)
        {
            _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            _port.DataReceived += _port_DataReceived;
            StatusPacketReceived += packet => Console.WriteLine(packet.ToString());
            Open();
        }

        readonly IList<byte> _buffer = new List<byte>();
        private bool _expectingHeader = true;
        void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[1];
            while (_port.BytesToRead > 0)
            {
                _port.Read(data, 0, 1);
                if (_expectingHeader)
                {
                    if(data[0] != 0xff) throw new Exception("header expected");
                    if (_buffer.Count == 1 && _buffer[0] == 0xff)
                    {
                        _expectingHeader = false;
                    }
                }

                _buffer.Add(data[0]);

                if (!BufferIsCompleteStatusPacket()) continue;

                if (StatusPacketReceived != null)
                {
                    StatusPacketReceived(new StatusPacket(_buffer));
                }
                _buffer.Clear();
                _expectingHeader = true;
            }
        }

        private bool BufferIsCompleteStatusPacket()
        {
            if (_buffer.Count < 4) return false;
            if (_buffer[0] != 0xff) return false;
            if (_buffer[1] != 0xff) return false;
            if (_buffer.Count != 4 + _buffer[3]) return false;
            return true;
        }

        //public StatusPacket ReadPacket()
        //{
        //    return new StatusPacket();
        //}


        public bool Open()
        {
            _port.Open();
            return _port.IsOpen;
        }

        public void Close()
        {
            _port.Close();
        }

        public void Write(IPacket packet)
        {
            var bytes = packet.ToByteArray();
            Console.WriteLine(packet.ToString());
            _port.Write(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            Close();
        }
    }

    public interface IPacket
    {
        byte CheckSum();
        byte[] ToByteArray();
        IEnumerable<byte> BuildPacket();
        
    }
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

    public enum Instruction:byte
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
