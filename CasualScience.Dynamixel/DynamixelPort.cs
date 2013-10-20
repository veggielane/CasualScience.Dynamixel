using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using CasualScience.Dynamixel.Packets;

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
            //StatusPacketReceived += packet => Console.WriteLine(packet.ToString());
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



}
