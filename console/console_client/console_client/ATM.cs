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
        public int stock {get; set; }
        public bool error { get; set; }

        public ATM(int atm_id, int stock, bool error)
        {
            this.atm_id = atm_id;
            this.stock = stock;
            this.error = error;
        }
    }
}
