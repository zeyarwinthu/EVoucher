using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Entity
{
    public class Transcation : CommonEntity
    {
        [Key]
        public int id { get; set; }
        public int evoucher_id { get; set; }
        public int payment_method_id { get; set; }
        public double amount { get; set; }
        public string user_country_code { get; set; }
        public string user_ph_no { get; set; }
    }
}
