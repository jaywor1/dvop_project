using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_client
{
    internal class Employe
    {
        public int id { get; set; }
        public int branch_id { get; set; }
        public string name { get; set; }

        public string position { get; set; }

        public bool present { get; set; }
    }
}
