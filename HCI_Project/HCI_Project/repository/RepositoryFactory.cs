using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.repository
{
    public class RepositoryFactory
    {
        public ClientRepository ClientRepository { get; }
        public DepartureRepository DepartureRepository { get; }
        public LineRepository LineRepository { get; }
        public ManagerRepository ManagerRepository { get; }
        public ReservationRepository ReservationRepository { get; }
        public SeatRepository SeatRepository { get; }
        public StationRepository StationRepository { get; }
        public TicketRepository TicketRepository { get; }
        public TrainRepository TrainRepository { get; }
        public WagonRepository WagonRepository { get; }

        public RepositoryFactory()
        {
            ClientRepository = new ClientRepository();
            DepartureRepository = new DepartureRepository();
            LineRepository = new LineRepository();
            ManagerRepository = new ManagerRepository();
            ReservationRepository = new ReservationRepository();
            SeatRepository = new SeatRepository();
            StationRepository = new StationRepository();
            TicketRepository = new TicketRepository();
            TrainRepository = new TrainRepository();
            WagonRepository = new WagonRepository();
        }
    }
}
