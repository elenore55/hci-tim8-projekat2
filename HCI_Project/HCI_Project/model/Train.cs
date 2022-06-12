using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public class Train : Serializable
    {
        public string Name { get; set; }
        public List<Wagon> Wagons { get; set; }

        public override string ToString()
        {
            int seats = 0;
            foreach (Wagon w in Wagons) seats += w.Seats.Count;
            return Name + " - " + Wagons.Count + " Wagons (" + seats + " Seats)"; 
        }
    }
}
