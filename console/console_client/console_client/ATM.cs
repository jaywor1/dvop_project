using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_client
{
    public class ATM
    {
        public int atm_id;
        public int stock;
        public bool error;
        public string address;


        public ATM(int atm_id, int stock, bool error, string address)
        {
            this.atm_id = atm_id;
            this.stock = stock;
            this.error = error;
            this.address = address;
        }
    }
}
