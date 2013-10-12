using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualScience.Dynamixel
{
    public class DynamixelBus:IDisposable
    {
        private DynamixelPort _port;

        public DynamixelBus(string portName, int baudRate)
        {
            _port = new DynamixelPort("COM3", 1000000);

        }

        public void Discover()
        {
            
        }

        public void Dispose()
        {
            _port.Dispose();
        }
    }
}
