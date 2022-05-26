using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public class Departure : Serializable
    {
        public DateTime StartDateTime { get; set; }
        public long LineId { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<Reservation> Reservations { get; set; }
        public Train Train { get; set; }
    }
}
