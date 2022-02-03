using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Request
{
    public class UserRequest
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string country_code { get; set; }
        public string ph_no { get; set; }
    }
}
