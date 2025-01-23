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
        public int xReal
        {
            get => xCoordinate + Radius;
            set => xCoordinate = value - Radius;
        }
        public int yReal
        {
            get => yCoordinate + Radius;
            set => yCoordinate = value - Radius;
        }
        public float Frequency { get; set; }
        public int Radius { get; set; }
        public int Distance { get; set; }
        public int Coof
        {
            get
            {
                var result =  (int)((((float)Distance / (float)Radius) * -1 + 1) * 100);
                if (result < 0)
                    result = 0;
                return result;
            }
        }
    }
}
