﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project.model
{
    public class Train : Serializable
    {
        public List<Wagon> Wagons { get; set; }
    }
}
