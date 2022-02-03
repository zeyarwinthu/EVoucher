using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPI.Model
{
    public class CommonEntity
    {
        public DateTime? created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public bool active_flag { get; set; }
    }
}
