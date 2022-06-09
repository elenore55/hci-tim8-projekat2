﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public class Line : Serializable
    {
        public double Price { get; set; }
        public List<Station> Stations { get; set; }
        // Koliko vremena voz putuje od stanice i - 1 do stanice i
        // Offsets[0] = 0
        public List<int> OffsetsInMinutes { get; set; }  
        public List<Departure> Departures { get; set; }

        public Station GetStartStation()
        {
            if (Stations == null || Stations.Count == 0) return null;
            return Stations[0];
        }

        public Station GetEndStation()
        {
            if (Stations == null || Stations.Count == 0) return null;
            return Stations[Stations.Count - 1];
        }

        public bool CanBeDeleted()
        {
            foreach(Departure d in Departures)
            {
                if (d.StartTime > DateTime.Now)
                {
                    if (d.Tickets.Count > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
