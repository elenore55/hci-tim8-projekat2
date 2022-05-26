using HCI_Project.model;

namespace HCI_Project.repository
{
    public class WagonRepository : GenericRepository<Wagon>
    {
        public WagonRepository()
        {
            path = "../../resources/wagons.xml";
            Load();
        }
    }
}
