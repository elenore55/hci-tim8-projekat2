using HCI_Project.model;

namespace HCI_Project.repository
{
    public class StationRepository : GenericRepository<Station>
    {
        public StationRepository()
        {
            path = "../../resources/stations.xml";
            Load();
        }
    }
}
