using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    [Authorize(Policy = "AuthorizationHeaderRequirement")]
    public class PurchaseController : ControllerBase
    {
        private readonly Appsetting _appSettings;
        private readonly IPurchaseBAL _PurchaseBAL;
        private readonly IPaymentMethodBAL _PaymentMethodBAL;
        public PurchaseController(IOptions<Appsetting> appsettings, IPurchaseBAL PurchaseBAL,IPaymentMethodBAL PaymentMethodBAL)
        {
            _appSettings = appsettings.Value;
            _PurchaseBAL = PurchaseBAL;
            _PaymentMethodBAL = PaymentMethodBAL;
        }
        [HttpGet("GetAllPaymentMethod")]
        public async Task<IActionResult> getAllPaymentMethod()
        {
            try
            {
                return Ok(await _PaymentMethodBAL.getAllPaymentMethod());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }
        }
        [HttpGet("PurchaseHistory")]
        public async Task<IActionResult> getAllPurchase(bool? status)
        {
            try
            {
                return Ok(await _PurchaseBAL.getAllPurchase(status));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }
        }
        [HttpPost("PurchaseEVoucher")]
        public async Task<IActionResult> purchaseEVoucher([FromBody] PurchaseRequest inputModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok(await _PurchaseBAL.purchaseEVoucher(inputModel));
                }
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.Error });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }
        }
        [HttpPost("VerifyPromoCode")]
        public async Task<IActionResult> VerifyPromoCode(string promo_code)
        {
            try
            {
                if (ModelState.IsValid)
                {
                     return Ok(await _PurchaseBAL.verifyPromoCode(promo_code));
                }
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.Error });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }
        }
        [HttpPost("CheckOut")]
        public async Task<IActionResult> checkOut(string promo_code,float shopping_amount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                     return Ok(await _PurchaseBAL.usePromoCode(promo_code, shopping_amount));
                }
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.Error });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }
        }
    }
}
