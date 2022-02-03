using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Authen
{
    public class UserAuthen
    {
        public static bool ValidateToken(string ControllerName)
        {
            var AllCanUse = GetAllCanUse();
            return AllCanUse.Contains(ControllerName.ToUpper());
        }
        private static List<string> GetAllCanUse()
        {
            return new List<string>() {

                "/api/v0.0/EVoucher/AddEVoucher".ToUpper(),
                "/api/v0.0/EVoucher/EditEVoucher".ToUpper(),
                "/api/v0.0/EVoucher/GetEVoucherList".ToUpper(),
                "/api/v0.0/EVoucher/GetEVoucherByID".ToUpper(),
                "/api/v0.0/User/RefreshToken".ToUpper(),
                "/api/v0.0/Purchase/PurchaseEVoucher".ToUpper(),
                "/api/v0.0/Purchase/GetAllPaymentMethod".ToUpper(),
                "/api/v0.0/Purchase/GetAllPurchase".ToUpper(),
                "/api/v0.0/Purchase/VerifyPromoCode".ToUpper(),
                "/api/v0.0/EVoucher/InActiveEVoucher".ToUpper(),
                "/api/v0.0/Purchase/PurchaseHistory".ToUpper(),
                "/api/v0.0/Purchase/CheckOut".ToUpper()
            };
        }
    }
}
