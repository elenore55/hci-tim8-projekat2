using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public class Line : Serializable
    {
        public double Price { get; set; }
        public List<Station> Stations { get; set; }
        // Koliko vremena voz putuje od stanice i - 1 do stanice i
        // Offsets[0] = 0
        public List<int> OffsetsInMinutes { get; set; }  
        public List<Departure> Departures { get; set; }
    }
}
