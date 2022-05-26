using HCI_Project.model;

namespace HCI_Project.repository
{
    public class ManagerRepository : GenericRepository<Manager>
    {
        public ManagerRepository()
        {
            path = "../../resources/managers.xml";
            Load();
        }

        public Manager GetByEmail(string email)
        {
            foreach (Manager manager in objects)
            {
                if (manager.Email == email) return manager;
            }
            return null;
        }
    }
}
