using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_client
{
    public class ATM
    {
        public int atm_id { get; set; }
        public int branch_id { get; set; }
        public int stock { get; set; }
        public string address { get; set; }


        public ATM(int atm_id, int branch_id, int stock, string address)
        {
            this.atm_id = atm_id;
            this.branch_id = branch_id;
            this.stock = stock;
            this.address = address;
        }
    }
}
