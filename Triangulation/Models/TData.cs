using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triangulation.Models
{
    public class Data
    {
        public int Id { get; set; }
        public Receiver Receiver { get; set; }
        public List<Router> Routers { get; set; }
    }
}
