using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_client
{
    internal class Employe
    {
        public int employe_id { get; set; }
        public int branch_id { get; set; }
        public string name { get; set; }

        public string position { get; set; }

        public bool present { get; set; }

        public Employe(int branch_id, string name, string position)
        {
            this.employe_id = 0;
            this.branch_id = branch_id;
            this.name = name;
            this.position = position;
            this.present = true;
        }
    }
}
