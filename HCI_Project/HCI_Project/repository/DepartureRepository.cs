using HCI_Project.model;

namespace HCI_Project.repository
{
    public class DepartureRepository : GenericRepository<Departure>
    {
        DepartureRepository()
        {
            path = "../../resources/departures.xml";
            Load();
        }
    }
}
