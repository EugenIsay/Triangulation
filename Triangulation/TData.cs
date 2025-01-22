using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triangulation.Models;

namespace Triangulation
{
    public static class TData
    {
        public static Receiver Receiver = new Receiver();
        public static List<Router> Routers = new List<Router>();
        public static void ChangeDictance()
        {
            foreach (Router router in Routers)
            {
                router.Distance = (int)Math.Sqrt(Math.Pow((router.xReal) - (Receiver.xCoordinate + 5), 2) + Math.Pow((router.yReal) - (Receiver.yCoordinate + 5), 2));
            }
        }
    }
}
