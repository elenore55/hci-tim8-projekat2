using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public class Ticket : Serializable
    {
        public DateTime PurchaseDateTime { get; set; }
        public string ClientEmail { get; set; }
        public long SeatId { get; set; }
        public long DepartureId { get; set; }
    }
}
