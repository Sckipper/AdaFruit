using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medic
{
    class SensorValue
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public String toString()
        {
            return "X:" + x + " Y:" + y + " Z:" + z;
        }

    }
}
