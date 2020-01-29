using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medic
{
    class SensorValue
    {
        public short x { get; set; }
        public short y { get; set; }
        public short z { get; set; }

        public String toString()
        {
            return "X:" + x + " Y:" + y + " Z:" + z;
        }
    }
}
