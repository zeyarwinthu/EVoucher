using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Request
{
    public class TranscationRequest
    {
        public int evoucher_id { get; set; }
        public int payment_method_id { get; set; }
        public CardInfo card_detail { get; set; }
        public double amount { get; set; }
        public string user_country_code { get; set; }
        public string user_ph_no { get; set; }
    }
    public class CardInfo
    {
        public string card_no { get; set; }
        public string card_name { get; set; }
        public string expiry_year { get; set; }
        public string expiry_month { get; set; }
        public string cvv { get; set; }
    }
}
