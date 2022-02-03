using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.BAL;
using WebAPI.Model;
using WebAPI.Model.Request;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("0.0")]
    [EnableCors("MyAllowSpecificOrigins")]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly Appsetting _appSettings;
        private readonly IUserBAL _userBAL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IOptions<Appsetting> appsettings,IUserBAL UserBAL, IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = appsettings.Value;
            _userBAL = UserBAL;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost("ShopAddUser")]
        public async Task<IActionResult> shopAddUser([FromBody] UserRequest inputModel)
        {
            try
            {
                int temp;
                if(string.IsNullOrEmpty(inputModel.first_name) || string.IsNullOrEmpty(inputModel.last_name) || string.IsNullOrEmpty(inputModel.country_code) || string.IsNullOrEmpty(inputModel.ph_no))
                {
                    return Ok(new ResponseModel { Message =Message.required_input, Status = APIStatus.Error });
                }
                else if(!int.TryParse(inputModel.country_code,out temp) || !int.TryParse(inputModel.ph_no,out temp))
                {
                    return Ok(new ResponseModel { Message = Message.mustbe_number, Status = APIStatus.Error });
                }
                else
                {
                    return Ok(await _userBAL.createUser(inputModel, UserRole.Shop));
                }
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [HttpPost("ShopRequestOTP")]
        public async Task<IActionResult> shopRequestOTP(string country_code,string ph_no)
        {
            try
            {
                return Ok(await _userBAL.shopRequestOTP(country_code, ph_no));
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [HttpPost("ShopVerifyOTP")]
        public async Task<IActionResult> shopVerifyOTP(string country_code,string ph_no,string otp)
        {
            try
            {
                return Ok(await _userBAL.shopVerifyOTP(country_code, ph_no,otp));
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [HttpPost("CustomerAddUser")]
        public async Task<IActionResult> customerAddUser([FromBody] UserRequest inputModel)
        {
            try
            {
                int temp;
                if (string.IsNullOrEmpty(inputModel.first_name) || string.IsNullOrEmpty(inputModel.last_name) || string.IsNullOrEmpty(inputModel.country_code) || string.IsNullOrEmpty(inputModel.ph_no))
                {
                    return Ok(new ResponseModel { Message = Message.required_input, Status = APIStatus.Error });
                }
                else if (!int.TryParse(inputModel.country_code, out temp) || !int.TryParse(inputModel.ph_no, out temp))
                {
                    return Ok(new ResponseModel { Message = Message.mustbe_number, Status = APIStatus.Error });
                }
                else
                {
                    return Ok(await _userBAL.createUser(inputModel, UserRole.User));
                }
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [HttpPost("CustomerRequestOTP")]
        public async Task<IActionResult> customerRequestOTP(string country_code, string ph_no)
        {
            try
            {
                return Ok(await _userBAL.customerRequestOTP(country_code, ph_no));
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [HttpPost("CustomerVerifyOTP")]
        public async Task<IActionResult> customerVerifyOTP(string country_code, string ph_no, string otp)
        {
            try
            {
                return Ok(await _userBAL.customerVerifyOTP(country_code, ph_no, otp));
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [Authorize(Policy = "AuthorizationHeaderRequirement")]
        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                var result = TKmgr.RefreshToken(claim);
                if (!string.IsNullOrEmpty(result))
                {
                    return Ok(new ResponseModel { Message = "Successful", Status = APIStatus.Successful, Data = result });
                }
                return Ok(new ResponseModel { Message = "Error", Status = APIStatus.Error });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel { Message = "SystemError", Status = APIStatus.SystemError, Data = ex.Message });
            }
        }
    }
}
