using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public class Message
    {
       
        public const string Successfully = "Successfully";

        public const string UpdateSucess = "Update Successfully";
        public const string UpdateFailed = "Update Failed.";

        public const string DeleteSucess = "Delete Successfully";
        public const string DeleteFailed = "Delete Failed";

        public const string AddSucess = "Add Successfully";
        public const string AddFailed = "Add Failed";

        public const string ErrorWhileFetchingData = "Error while fetching data.";
        public const string InvalidPostedData = "Posted invalid data.";
        public const string NoData = "No data was found";
        public const string Result = "Result Found!";

        public const string phExist = "Phone number already exist!";

        public const string NoEmail = "Email not found!";
        public const string PasswordError = "Incorrect password!";
        public const string c_SystemError = "System Error, Please Contract Administrator.";


        public const string required_input = "Please fill all required field!";
        public const string mustbe_number = "Country code and phone number must be number!";

        public const string contectAdmin = "Your phone number is disabled. Please Contact Administrator!";
        public const string notRegister = "Your number is not register yet!";

        public const string wrongOTP = "Your OTP is not correct!";

        public const string notAdmin = "You are not admin!";
        public const string notCustomer = "You are not customer!";

        public const string noPermission = "You don't have permission!";
        public const string overMax = "You can't buy over eVoucher maximum limit ";
        public const string overQuantity = "You can't buy over total quantity of eVoucher!";
        public const string notGift = "This eVoucher is not for Gift!";
        public const string Gift = "This eVoucher is for Gift!";
        public const string notValidEVoucher = "This eVoucher is not valid!";
        public const string wrongPromo_code = "This is wrong eVoucher code!";
        public const string wrongUser = "EVoucher code is not match with current user!";
        public const string expiredPromo = "EVoucher code is expired!";
        public const string inActive = "EVoucher code is inActive!";
        public const string alreadyUsed = "EVoucher code is already used!";
        public const string exceedAmount = "Your shopping amount is exceed in eVoucher!";
    }
}
