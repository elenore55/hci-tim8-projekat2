using HCI_Project.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Project
{
    public class DemoProvider
    {
        public static void ShowDemo(string pageTitle) 
        {
            DemoPlayer m = new DemoPlayer($@"../../videos/{ pageTitle }.mkv");
            m.ShowDialog();
        }
    }
}
