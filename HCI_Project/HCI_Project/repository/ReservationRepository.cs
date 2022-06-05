using HCI_Project.model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HCI_Project.repository
{
    public class ReservationRepository : GenericRepository<Reservation>
    {
        public ReservationRepository()
        {
            path = "../../resources/reservations.xml";
            Load();
        }

        public List<Reservation> GetByClient(string email)
        {
            return (from reservation in objects where reservation.ClientEmail == email && reservation.IsActive select reservation).ToList();
        }

        public List<Reservation> GetByClient(string email, DateTime start, DateTime end)
        {
            if (DateTime.Compare(end, DateTime.MaxValue) != 0)
            {
                end = end.AddDays(1);
            }
            return (from reservation in objects 
                    where reservation.ClientEmail == email && reservation.IsActive &&
                    DateTime.Compare(reservation.ReservationDateTime, start) >= 0 && DateTime.Compare(reservation.ReservationDateTime, end) < 0
                    select reservation).ToList();
        }
    }
}
