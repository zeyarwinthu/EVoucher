using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Response
{
    public class EVoucherResponse
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public double amount { get; set; }
        public int quantity { get; set; }
        public DateTime expired_date { get; set; }
        public string status { get; set; }
    }
    public class EVoucherDetails
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public double amount { get; set; }
        public List<int> payment_method { get; set; }
        public int quantity { get; set; }
        public string buy_type { get; set; }
        public DateTime expired_date { get; set; }
        public int? discount_payment_id { get; set; }
        public double? discount_price { get; set; }
        public int max_buy { get; set; }
        public string status { get; set; }
    }
}
