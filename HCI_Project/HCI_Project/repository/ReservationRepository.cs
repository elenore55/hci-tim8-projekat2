using HCI_Project.model;
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
            return (from reservation in objects where reservation.ClientEmail == email select reservation).ToList();
        }
    }
}
