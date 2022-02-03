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
    public class EVoucherController : ControllerBase
    {
        private readonly Appsetting _appSettings;
        private readonly IEVoucherBAL _EVoucherBAL;
        public EVoucherController(IOptions<Appsetting> appsettings, IEVoucherBAL EVoucherBAL)
        {
            _appSettings = appsettings.Value;
            _EVoucherBAL = EVoucherBAL;
        }
        [HttpPost("AddEVoucher")]
        public async Task<IActionResult> addEVoucher([FromBody] EVoucherRequest inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.title) || inputModel.amount <= 0.0 || inputModel.payment_method.Count < 1
                    || inputModel.quantity < 1 || inputModel.buy_type < 1 || inputModel.max_buy < 1)
                {
                    return Ok(new ResponseModel { Message = Message.required_input, Status = APIStatus.Error });
                }
                else
                {
                    return Ok(await _EVoucherBAL.createEVoucher(inputModel));
                }
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [HttpPost("EditEVoucher")]
        public async Task<IActionResult> editEVoucher([FromBody] UpdateEVoucher inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.title) || inputModel.amount <= 0.0 || inputModel.payment_method.Count < 1 
                    || inputModel.quantity < 1 || inputModel.buy_type < 1 || inputModel.max_buy < 1)
                {
                    return Ok(new ResponseModel { Message = Message.required_input, Status = APIStatus.Error });
                }
                else
                {
                    return Ok(await _EVoucherBAL.updateEVoucher(inputModel));
                }
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [HttpGet("GetEVoucherList")]
        public async Task<IActionResult> getEVoucherList()
        {
            try
            {
                return Ok(await _EVoucherBAL.getEVoucherList());
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [HttpGet("GetEVoucherByID")]
        public async Task<IActionResult> getEVoucherByID(int id)
        {
            try
            {
                return Ok(await _EVoucherBAL.getEVoucherByID(id));
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
        [HttpPost("InActiveEVoucher")]
        public async Task<IActionResult> inActiveEVoucher(int id)
        {
            try
            {
                return Ok(await _EVoucherBAL.inActiveEVoucher(id));
            }
            catch (Exception Ex)
            {
                return Ok(new ResponseModel { Message = Message.c_SystemError, Status = APIStatus.SystemError });
            }

        }
    }
}
