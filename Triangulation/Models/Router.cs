using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triangulation.Models
{
    public class Router
    {
        public int Id { get; set; }
        public int xCoordinate { get; set; }
        public int yCoordinate { get; set; }
        public float Frequency { get; set; }
        public int Radius { get; set; }
        public float Distance { get; set; }

    }
}
