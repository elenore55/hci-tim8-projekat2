using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public class Client : User
    {
        public List<Ticket> Tickets { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}
