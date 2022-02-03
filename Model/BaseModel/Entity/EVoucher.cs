using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Entity
{
    public class EVoucher : CommonEntity
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public double amount { get; set; }
        public string payment_method_id { get; set; }
        public int quantity { get; set; }
        public byte buy_type { get; set; }
        public DateTime expired_date { get; set; }
        public int? discount_payment_id { get; set; }
        public double? discount_price { get; set; }
        public int max_buy { get; set; }
        public bool status { get; set; }
    }
}
