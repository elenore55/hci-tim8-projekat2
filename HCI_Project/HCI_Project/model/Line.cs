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
        public List<Departure> Departures { get; set; }
    }
}
