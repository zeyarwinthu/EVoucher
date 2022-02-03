using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Response
{
    public class PurchaseResponse
    {
        public int id { get; set; }
        public string promo_code { get; set; }
        public int evoucher_id { get; set; }
        public Guid user_id { get; set; }
        public string user_country_code { get; set; }
        public string user_ph_no { get; set; }
        public string qr_code { get; set; }
    }
    public class CheckOutResponse
    {
        public int id { get; set; }
        public string promo_code { get; set; }
        public string qr_code { get; set; }
    }
}
