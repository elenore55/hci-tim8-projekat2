using HCI_Project.model;

namespace HCI_Project.repository
{
    public class LineRepository : GenericRepository<Line>
    {
        public LineRepository()
        {
            path = "../../resources/lines.xml";
            Load();
        }
    }
}
