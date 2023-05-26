using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_client
{
    internal class Branch
    {
        public int branch_id { get; set; }
        public string open_hours { get; set; }
        public string close_hours { get; set; }
        public string address { get; set; }

        public Branch(int branch_id, string open_hours, string close_hours, string address)
        {
            this.branch_id = branch_id;
            this.open_hours = open_hours;
            this.close_hours = close_hours;
            this.address = address;
        }
    }
}
