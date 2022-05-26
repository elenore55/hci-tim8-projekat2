using HCI_Project.model;

namespace HCI_Project.repository
{
    public class TicketRepository : GenericRepository<Ticket>
    {
        public TicketRepository()
        {
            path = "../../resources/tickets.xml";
            Load();
        }
    }
}
