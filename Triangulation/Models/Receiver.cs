using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triangulation.Models
{
    public class Receiver
    {
        public float Frequency { get; set; }
        public int xCoordinate { get; set; }
        public int yCoordinate { get; set; }
        public int xReal
        {
            get => xCoordinate + 5;
            set => xCoordinate = value - 5;
        }
        public int yReal
        {
            get => yCoordinate + 5;
            set => yCoordinate = value - 5;
        }
    }
}
