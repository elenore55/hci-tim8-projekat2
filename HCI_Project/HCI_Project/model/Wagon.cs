using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public enum WagonClass 
    {
        First, Second
    }

    public class Wagon : Serializable
    {
        public WagonClass Class { get; set; }
        public int Ordinal { get; set; }
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
        public List<Seat> Seats { get; set; }
    }
}
