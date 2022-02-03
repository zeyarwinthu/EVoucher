using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPI.Model
{
    public class Appsetting
    {
        public string ConnectionStrings { get; set; }

        public string Secret { get; set; }

        public string Environment { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

    }
    public enum UserRole
    {
        User,Shop
    }
    public class Token
    {
        public string userid { get; set; }
        public string country_code { get; set; }
        public string ph_no { get; set; }
        public string user_role { get; set; }
    }
}
