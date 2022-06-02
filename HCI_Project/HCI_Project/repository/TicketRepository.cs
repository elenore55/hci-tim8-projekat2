using HCI_Project.model;
using System.Collections.Generic;
using System.Linq;

namespace HCI_Project.repository
{
    public class TicketRepository : GenericRepository<Ticket>
    {
        public TicketRepository()
        {
            path = "../../resources/tickets.xml";
            Load();
        }

        public List<Ticket> GetByClient(string email)
        {
            return (from ticket in objects where ticket.ClientEmail == email select ticket).ToList();
        }
    }
}
