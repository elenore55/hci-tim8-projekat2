﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public class Reservation
    {
        public long Id { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public bool IsActive { get; set; }
        public string ClientEmail { get; set; }
        public long SeatId { get; set; }
        public long DepartureId { get; set; }
    }
}
