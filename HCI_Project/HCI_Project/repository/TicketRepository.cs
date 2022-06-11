using HCI_Project.model;
using System;
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

        public List<Ticket> GetByClient(string email, DateTime start, DateTime end)
        {
            if (DateTime.Compare(end, DateTime.MaxValue) != 0)
            {
                end = end.AddDays(1);
            }
            return (from ticket in objects 
                    where ticket.ClientEmail == email && 
                    DateTime.Compare(ticket.PurchaseDateTime, start) >= 0 && DateTime.Compare(ticket.PurchaseDateTime, end) < 0  
                    select ticket).ToList();
        }

        internal List<Ticket> GetByMonth(int selectedMonth, int selectedYear)
        {
            selectedMonth++;        // bilo je neophodno
            List<Ticket> retVal = new List<Ticket>();
            foreach (Ticket t in objects)
            {
                if (t.PurchaseDateTime.Year==selectedYear && t.PurchaseDateTime.Month==selectedMonth)
                {
                    retVal.Add(t);
                }
            }
            return retVal;
        }

        internal List<Ticket> GetByDepartureId(long id, DateTime selectedDate)
        {
            List<Ticket> retVal = new List<Ticket>();
            foreach (Ticket t in objects)
            {
                if (t.DepartureId==id && t.DepartureDate==selectedDate)
                {
                    retVal.Add(t);
                }
            }
            return retVal;
        }
    }
}
