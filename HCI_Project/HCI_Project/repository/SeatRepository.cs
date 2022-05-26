using HCI_Project.model;

namespace HCI_Project.repository
{
    public class SeatRepository : GenericRepository<Seat>
    {
        public SeatRepository()
        {
            path = "../../resources/seats.xml";
            Load();
        }
    }
}
