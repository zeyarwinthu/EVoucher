using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Request
{
    public class PurchaseRequest
    {
        public int evoucher_id { get; set; }
        public string gift_country_code { get; set; }
        public string gift_ph_no { get; set; }
        public int payment_method_id { get; set; }
        public CardInfo card_detail { get; set; }
        public int quantity { get; set; }

    }
}
