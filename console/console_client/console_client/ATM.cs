using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_client
{
    public class ATM
    {
        public long atm_id { get; set; }
        public long stock {get; set; }
        public string? withdraw_log { get; set; }

        public string? error_log { get; set; }
    }
}
