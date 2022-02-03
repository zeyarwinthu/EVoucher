using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Entity
{
    public class User : CommonEntity
    {
        [Key]
        public Guid id { get; set; }
        public string role { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string country_code { get; set; }
        public string ph_no { get; set; }
        public string otp { get; set; }
    }
}
