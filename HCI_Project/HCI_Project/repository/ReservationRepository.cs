using HCI_Project.model;

namespace HCI_Project.repository
{
    public class ReservationRepository : GenericRepository<Reservation>
    {
        ReservationRepository()
        {
            path = "../../resources/reservations.xml";
            Load();
        }
    }
}
