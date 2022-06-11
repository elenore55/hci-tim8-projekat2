using HCI_Project.model;
using System;
using System.Collections.Generic;

namespace HCI_Project.repository
{
    public class LineRepository : GenericRepository<Line>
    {
        public LineRepository()
        {
            path = "../../resources/lines.xml";
            Load();
        }

        public List<Line> FilterLines(string from, string to)
        {
            List<Line> result = new List<Line>();
            foreach (Line line in objects)
            {
                bool startFound = false;
                bool endFound = false;
                foreach (Station s in line.Stations)
                {
                    if (s.Name == from)
                    {
                        startFound = true;
                    }
                    else if (s.Name == to && startFound)
                    {
                        endFound = true;
                        break;
                    }
                }
                if (endFound)
                {
                    result.Add(line);
                }
            }
            return result;
        }

        internal List<Line> FilterLinesEndpoints(string from, string to)
        {
            List<Line> result = new List<Line>();
            foreach (Line line in objects)
            {
                if (line.Stations[0].Name.Equals(from) && line.Stations[line.Stations.Count - 1].Name.Equals(to))
                {
                    result.Add(line);
                }
            }
            return result;
        }
    }
}
