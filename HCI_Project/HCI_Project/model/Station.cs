using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public enum StationType
    {
        Start, Middle, End
    }

    public class Station : Serializable
    {
        public string Name { get; set; }
        public Point Coords { get; set; }
        public StationType Type { get; set; }
        public long LineId { get; set; }
    }
}
