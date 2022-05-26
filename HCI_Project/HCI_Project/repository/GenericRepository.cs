using HCI_Project.model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace HCI_Project.repository
{
    public class GenericRepository<T> where T : Serializable
    {
        protected long generator;
        protected List<T> objects;
        protected string path;

        public GenericRepository() 
        {
            generator = 0;
        }

        public void Add(T obj)
        {
            objects.Add(obj);
            Load();
            SaveAll();
        }

        public void Load()
        {
            XmlSerializer x = new XmlSerializer(objects.GetType());
            StreamReader reader = new StreamReader(path);
            objects = (List<T>)x.Deserialize(reader);
            GetLastId();
        }

        public void Delete(long id)
        {
            T toRemove = objects.SingleOrDefault(x => x.Id == id);
            if (toRemove != null)
            {
                objects.Remove(toRemove);
                SaveAll();
                GetLastId();
            }
        }

        public void SaveAll()
        {
            XmlSerializer x = new XmlSerializer(objects.GetType());
            StreamWriter writer = new StreamWriter(path);
            x.Serialize(writer, objects);
        }

        public long GetNextId()
        {
            return ++generator;
        }

        public T GetById(long id)
        {
            foreach (T obj in objects)
            {
                if (obj.Id == id) return obj;
            }
            return null;
        }

        protected void GetLastId()
        {
            foreach (T obj in objects)
            {
                if (obj.Id >= generator)
                {
                    generator = obj.Id;
                }
            }
        }
    }
}
