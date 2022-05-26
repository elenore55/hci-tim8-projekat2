using HCI_Project.model;

namespace HCI_Project.repository
{
    public class DepartureRepository : GenericRepository<Departure>
    {
        public DepartureRepository()
        {
            path = "../../resources/departures.xml";
            Load();
        }
    }
}
