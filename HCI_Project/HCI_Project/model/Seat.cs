using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public enum SeatStatus
    {
        Free, Reserved, Taken
    }

    public class Seat
    {
        public long Id { get; set; }
        public SeatStatus Status { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public long WagonId { get; set; }
    }
}
