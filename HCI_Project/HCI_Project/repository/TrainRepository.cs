using HCI_Project.model;

namespace HCI_Project.repository
{
    public class TrainRepository : GenericRepository<Train>
    {
        public TrainRepository()
        {
            path = "../../resources/trains.xml";
            Load();
        }
    }
}
