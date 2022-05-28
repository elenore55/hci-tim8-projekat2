using HCI_Project.model;

namespace HCI_Project.repository
{
    public class ClientRepository : GenericRepository<Client>
    {
        public ClientRepository()
        {
            path = "../../resources/clients.xml";
            Load();
        }

        public Client GetByEmail(string email)
        {
            foreach (Client client in objects)
            {
                if (client.Email == email) return client;
            }
            return null;
        }

        public Client GetByUsername(string username)
        {
            foreach (Client client in objects)
            {
                if (client.Username == username) return client;
            }
            return null;
        }
    }
}
