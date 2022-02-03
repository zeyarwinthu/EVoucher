using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Entity
{
    public class Purchase:CommonEntity
    {
        [Key]
        public int id { get; set; }
        public string promo_code { get; set; }
        public int evoucher_id { get; set; }
        public Guid user_id { get; set; }
        public string user_country_code { get; set; }
        public string user_ph_no { get; set; }
        public string gift_country_code { get; set; }
        public string gift_ph_no { get; set; }
        public DateTime? used_date { get; set; }
        public bool status { get; set; }
    }
}
