using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model.Entity
{
    public class PaymentMethod
    {
        [Key]
        public int id { get; set; }
        public string method { get; set; }
    }
}
