using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public class PromoCode
    {
        public string generatePromoCode()
        {
            try
            {
                Random random = new Random();
                var dig = "0123456789";
                var dig6 = new string(Enumerable.Repeat(dig, 6).Select(s => s[random.Next(s.Length)]).ToArray());
                var alp = "abcdefghijklmnopqustuvwxyzABCDEFGHIJKLMNOPQUSTUVWXYZ";
                var alp5 = new string(Enumerable.Repeat(alp, 5).Select(s => s[random.Next(s.Length)]).ToArray());
                var tmpString = dig6 + alp5;
                var promoCode = new string(tmpString.ToCharArray().OrderBy(s => (random.Next(2) % 2) == 0).ToArray());
                return promoCode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
