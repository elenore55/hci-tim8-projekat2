using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public class Station : Serializable
    {
        public string Name { get; set; }
        public System.Windows.Point Coords { get; set; }

        public Station() { }
        public Station(System.Windows.Point p, string name)
        {
            Coords = p;
            Name = name;
        }
    }
}
