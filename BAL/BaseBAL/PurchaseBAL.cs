using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Model;
using WebAPI.Model.Entity;
using WebAPI.Model.Request;
using WebAPI.Model.Response;
using WebAPI.Repository;

namespace WebAPI.BAL
{
    public interface IPurchaseBAL : IGenericBAL<Purchase>
    {
        Task<ResponseModel> purchaseEVoucher(PurchaseRequest inputModel);
        Task<ResponseModel> getAllPurchase(bool? status);
        Task<ResponseModel> verifyPromoCode(string promo_code);
        Task<ResponseModel> usePromoCode(string promo_code, double shopping_amount);
    }
    public class PurchaseBAL : GenericBAL<Purchase>, IPurchaseBAL
    {
        private readonly Appsetting _appSettings;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Purchase> _PurchaseRepository;
        private readonly IGenericRepository<EVoucher> _EVoucherRepository;
        private readonly ITranscationBAL _TranscationBAL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentMethodBAL _PaymentMethodBAL;
        
        public PurchaseBAL(IGenericRepository<Purchase> PurchaseRepository, IOptions<Appsetting> appsettings,ITranscationBAL TranscationBAL,
            IHttpContextAccessor httpContextAccessor,IMapper mapper,IGenericRepository<EVoucher> EVoucherRepository, IPaymentMethodBAL PaymentMethodBAL) : base(PurchaseRepository)
        {
            _PurchaseRepository = PurchaseRepository;
            _mapper = mapper;
            _appSettings = appsettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _EVoucherRepository = EVoucherRepository;
            _TranscationBAL = TranscationBAL;
            _PaymentMethodBAL = PaymentMethodBAL;
        }
        public async Task<ResponseModel> purchaseEVoucher(PurchaseRequest inputModel)
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) && claim.user_role == UserRole.User.ToString())
                {
                    var evoucher = _EVoucherRepository.GetByExp(x => x.active_flag == true && x.status == true && x.id == inputModel.evoucher_id && x.expired_date > DateTime.UtcNow).FirstOrDefault();
                    if (evoucher != null)
                    {
                        var user_purchase = _PurchaseRepository.GetByExp(x => x.active_flag == true && x.user_id == Guid.Parse(claim.userid) && x.evoucher_id == inputModel.evoucher_id).ToList();
                        var total_purchase= _PurchaseRepository.GetByExp(x => x.active_flag == true && x.evoucher_id == inputModel.evoucher_id).ToList();
                        if (user_purchase.Count + inputModel.quantity > evoucher.max_buy)
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.overMax };
                        }
                        else if(total_purchase.Count + inputModel.quantity > evoucher.quantity)
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.overQuantity };
                        }
                        else if(evoucher.buy_type == 1 && !string.IsNullOrEmpty(inputModel.gift_country_code) && !string.IsNullOrEmpty(inputModel.gift_ph_no))
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.notGift };
                        }
                        else if(evoucher.buy_type != 1 && string.IsNullOrEmpty(inputModel.gift_country_code) && string.IsNullOrEmpty(inputModel.gift_ph_no))
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.Gift };
                        }
                        else
                        {
                            double amount = evoucher.amount;
                            if(evoucher.discount_payment_id.HasValue && evoucher.discount_payment_id == inputModel.payment_method_id && evoucher.discount_price.HasValue)
                            {
                                amount = ((100 - evoucher.discount_price.Value)/100) * evoucher.amount;
                            }
                            TranscationRequest tran = new TranscationRequest
                            {
                                evoucher_id = inputModel.evoucher_id,
                                payment_method_id = inputModel.payment_method_id,
                                card_detail = inputModel.card_detail,
                                amount = inputModel.quantity * amount,
                                user_country_code = claim.country_code,
                                user_ph_no = claim.ph_no
                            };
                            bool dbAdd = await _TranscationBAL.CreateTranscation(tran);
                            if (!dbAdd)
                            {
                                return new ResponseModel() { Status = APIStatus.Error, Message = Message.AddFailed };
                            }
                            PromoCode promo = new PromoCode();
                            List<Purchase> pur_list = new List<Purchase>();
                            for (int i = 0; i < inputModel.quantity; i++)
                            {
                                var promo_code = "";
                                do
                                {
                                    promo_code = promo.generatePromoCode();
                                } while (_PurchaseRepository.GetByExp(x => x.promo_code == promo_code).FirstOrDefault() != null && pur_list.Exists(x => x.promo_code == promo_code));
                                if (!string.IsNullOrEmpty(promo_code))
                                {
                                    Purchase tmp = new Purchase
                                    {
                                        promo_code = promo_code,
                                        evoucher_id = inputModel.evoucher_id,
                                        user_id = Guid.Parse(claim.userid),
                                        user_country_code = claim.country_code,
                                        user_ph_no = claim.ph_no,
                                        gift_country_code = inputModel.gift_country_code,
                                        gift_ph_no = inputModel.gift_ph_no,
                                        status = false,
                                        active_flag = true,
                                        created_date = DateTime.UtcNow
                                    };
                                    pur_list.Add(tmp);
                                }
                            }
                            dbAdd = await _PurchaseRepository.CreateRange(pur_list);
                            if (dbAdd)
                            {
                                QRCodeCreater qr = new QRCodeCreater();
                                List<CheckOutResponse> chData = pur_list.Select(x => new CheckOutResponse { id = x.id, promo_code = x.promo_code, qr_code = qr.CreateQRCode(x.promo_code + "\n" + x.user_country_code + x.user_ph_no) }).ToList();
                                return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully, Data= chData };
                            }
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.AddFailed };
                        }
                    }
                    return new ResponseModel() { Status = APIStatus.Error, Message = Message.notValidEVoucher };
                }
                return new ResponseModel() { Status = APIStatus.Error, Message = Message.noPermission };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }
        public async Task<ResponseModel> getAllPurchase(bool? status)
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) && 
                    (claim.user_role == UserRole.User.ToString() || claim.user_role == UserRole.Shop.ToString()))
                {
                    QRCodeCreater qr = new QRCodeCreater();
                    var purch_list = _PurchaseRepository.GetByExp(x => x.active_flag == true).ToList();
                    List<PurchaseResponse> returnData = (from p in purch_list
                                                         where claim.user_role==UserRole.User.ToString()?p.user_id==Guid.Parse(claim.userid):true &&
                                                         status!=null?p.status==status:true &&
                                                         claim.user_role!=UserRole.Shop.ToString()?p.user_id==Guid.Parse(claim.userid):true
                                                         orderby p.id descending
                                                         select new PurchaseResponse()
                                                         {
                                                             id = p.id,
                                                             promo_code = p.status ? null : p.promo_code,
                                                             evoucher_id = p.evoucher_id,
                                                             user_id = p.user_id,
                                                             user_country_code = p.user_country_code,
                                                             user_ph_no = p.user_ph_no,
                                                             qr_code = p.status ? null : qr.CreateQRCode(p.promo_code + "\n" + p.user_country_code + p.user_ph_no)
                                                         }).ToList();
                    return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully, Data= returnData };
                }
                return new ResponseModel() { Status = APIStatus.Error, Message = Message.noPermission };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }
        public async Task<ResponseModel> verifyPromoCode(string promo_code)
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) && claim.user_role == UserRole.User.ToString())
                {
                    var purchase_data = _PurchaseRepository.GetByExp(x => x.active_flag == true && x.promo_code == promo_code ).FirstOrDefault();
                    if (purchase_data != null)
                    {
                        if (purchase_data.status)
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.alreadyUsed };
                        }
                        var eVoucher = _EVoucherRepository.GetByExp(x => x.id == purchase_data.evoucher_id).FirstOrDefault();
                        if (eVoucher == null)
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.notValidEVoucher };
                        }
                        if(!eVoucher.active_flag || eVoucher.expired_date < DateTime.UtcNow)
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.expiredPromo };
                        }
                        if (!eVoucher.status)
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.inActive };
                        }
                        if(purchase_data.gift_ph_no!=null && (claim.ph_no!=purchase_data.gift_ph_no && claim.country_code== purchase_data.gift_country_code))
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.wrongUser };
                        }
                        if(purchase_data.gift_ph_no==null && claim.ph_no!=purchase_data.user_ph_no && claim.country_code== purchase_data.user_country_code)
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.wrongUser };
                        }
                        return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully };
                    }
                    return new ResponseModel() { Status = APIStatus.Error, Message = Message.wrongPromo_code };
                }
                return new ResponseModel() { Status = APIStatus.Error, Message = Message.noPermission };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }
        public async Task<ResponseModel> usePromoCode(string promo_code,double shopping_amount)
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) && claim.user_role == UserRole.User.ToString())
                {
                    var promo_data = _PurchaseRepository.GetByExp(x => x.active_flag == true && x.promo_code == promo_code).FirstOrDefault();
                    if (promo_data != null)
                    {
                        var eVoucher = _EVoucherRepository.GetByExp(x => x.active_flag == true && x.id == promo_data.evoucher_id).FirstOrDefault();
                        if (shopping_amount > eVoucher.amount)
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.exceedAmount };
                        }
                        promo_data.status = true;
                        promo_data.used_date = DateTime.UtcNow;
                        promo_data.updated_date = DateTime.UtcNow;
                        var obj = await _PurchaseRepository.Update(promo_data);
                        if (obj)
                        {
                            return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully };
                        }
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.UpdateFailed };
                    }
                    return new ResponseModel() { Status = APIStatus.Error, Message = Message.notValidEVoucher };
                }
                return new ResponseModel() { Status = APIStatus.Error, Message = Message.noPermission };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }

    }
}
