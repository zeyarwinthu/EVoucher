using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Model;
using WebAPI.Model.Entity;
using WebAPI.Repository;

namespace WebAPI.BAL
{
    public interface IPaymentMethodBAL : IGenericBAL<PaymentMethod>
    {
        Task<ResponseModel> getAllPaymentMethod();
    }
    public class PaymentMethodBAL : GenericBAL<PaymentMethod>, IPaymentMethodBAL
    {
        private readonly Appsetting _appSettings;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<PaymentMethod> _PaymentMethodRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PaymentMethodBAL(IGenericRepository<PaymentMethod> PaymentMethodRepository, IHttpContextAccessor httpContextAccessor,
            IOptions<Appsetting> appsettings, IMapper mapper) : base(PaymentMethodRepository)
        {
            _PaymentMethodRepository = PaymentMethodRepository;
            _mapper = mapper;
            _appSettings = appsettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseModel> getAllPaymentMethod()
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) &&
                    (claim.user_role == UserRole.Shop.ToString() || claim.user_role == UserRole.User.ToString()))
                {
                    var payment_list = _PaymentMethodRepository.GetByExp(x => x.id != null).ToList();
                    return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully, Data = payment_list };
                }
                else
                {
                    return new ResponseModel() { Status = APIStatus.Error, Message = Message.noPermission };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }
    }
}
