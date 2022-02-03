using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
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
    public interface IEVoucherBAL : IGenericBAL<EVoucher>
    {
        Task<ResponseModel> createEVoucher(EVoucherRequest inputModel);
        Task<ResponseModel> updateEVoucher(UpdateEVoucher inputModel);
        Task<ResponseModel> getEVoucherList();
        Task<ResponseModel> getEVoucherByID(int id);
        Task<ResponseModel> inActiveEVoucher(int id);
    }
    public class EVoucherBAL : GenericBAL<EVoucher>, IEVoucherBAL
    {
        private readonly Appsetting _appSettings;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<EVoucher> _EVoucherRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EVoucherBAL(IGenericRepository<EVoucher> EVoucherRepository, IHttpContextAccessor httpContextAccessor,
            IOptions<Appsetting> appsettings, IMapper mapper) : base(EVoucherRepository)
        {
            _EVoucherRepository = EVoucherRepository;
            _mapper = mapper;
            _appSettings = appsettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseModel> createEVoucher(EVoucherRequest inputModel)
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) && claim.user_role == UserRole.Shop.ToString())
                {
                    var obj = _mapper.Map<EVoucher>(inputModel);
                    obj.payment_method_id = string.Join(",", inputModel.payment_method);
                    obj.status = true;
                    obj.created_date = DateTime.UtcNow;
                    obj.active_flag = true;
                    var addEVoucher = await _EVoucherRepository.Create(obj);
                    if (addEVoucher)
                    {
                        BackgroundJob.Schedule(() => ExpiredEVoucher(obj.id), obj.expired_date);
                        return new ResponseModel() { Status = APIStatus.Successful, Message = Message.AddSucess };
                    }
                    else
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.AddFailed };
                    }
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
        public async Task<ResponseModel> updateEVoucher(UpdateEVoucher inputModel)
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) && claim.user_role == UserRole.Shop.ToString())
                {
                    var oldData = _EVoucherRepository.GetByExp(x => x.active_flag == true && x.id == inputModel.id).FirstOrDefault();
                    if (oldData != null)
                    {
                        oldData.title = inputModel.title;
                        oldData.description = inputModel.description;
                        oldData.image = inputModel.image;
                        oldData.amount = inputModel.amount;
                        oldData.payment_method_id = string.Join(", ", inputModel.payment_method);
                        oldData.quantity = inputModel.quantity;
                        oldData.buy_type = inputModel.buy_type;
                        oldData.expired_date = inputModel.expired_date;
                        oldData.discount_payment_id = inputModel.discount_payment_id;
                        oldData.discount_price = inputModel.discount_price;
                        oldData.max_buy = inputModel.max_buy;
                        oldData.updated_date = DateTime.UtcNow;
                        var updateData = await _EVoucherRepository.Update(oldData);
                        if (updateData)
                        {
                            return new ResponseModel() { Status = APIStatus.Successful, Message = Message.UpdateSucess };
                        }
                        else
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.UpdateFailed };
                        }
                    }
                    else
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.ErrorWhileFetchingData };
                    }
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
        public async Task<ResponseModel> getEVoucherList()
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) && 
                    (claim.user_role == UserRole.Shop.ToString() || claim.user_role==UserRole.User.ToString()))
                {
                    var voucher_list = _EVoucherRepository.GetByExp(x => x.active_flag == true).ToList();
                    List<EVoucherResponse> tmpList = new List<EVoucherResponse>();
                    foreach (var item in voucher_list)
                    {
                        EVoucherResponse tmp = new EVoucherResponse
                        {
                            id = item.id,
                            title = item.title,
                            description = item.description,
                            image = item.image,
                            amount = item.amount,
                            quantity = item.quantity,
                            expired_date = item.expired_date,
                            status = item.status ? "Active" : "InActive"
                        };
                        tmpList.Add(tmp);
                    }
                    return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully, Data = tmpList };
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
        public async Task<ResponseModel> getEVoucherByID(int id)
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) &&
                    (claim.user_role == UserRole.Shop.ToString() || claim.user_role == UserRole.User.ToString()))
                {
                    var voucher_data = _EVoucherRepository.GetByExp(x => x.active_flag == true && x.id == id).FirstOrDefault();
                    EVoucherDetails tmp = new EVoucherDetails();
                    if (voucher_data != null)
                    {
                        var methods = !string.IsNullOrEmpty(voucher_data.payment_method_id) ? voucher_data.payment_method_id.Split(',') : null;
                        var int_methods = methods.Select(int.Parse).ToList();
                        tmp = new EVoucherDetails
                        {
                            id = voucher_data.id,
                            title = voucher_data.title,
                            description = voucher_data.description,
                            image = voucher_data.image,
                            amount = voucher_data.amount,
                            payment_method = int_methods,
                            quantity = voucher_data.quantity,
                            buy_type = voucher_data.buy_type == 1 ? "only me usage" : "gift to others",
                            expired_date =voucher_data.expired_date,
                            discount_payment_id=voucher_data.discount_payment_id,
                            discount_price=voucher_data.discount_price,
                            max_buy=voucher_data.max_buy,
                            status=voucher_data.status ? "Active" : "InActive"
                        };
                    }
                    else
                    {
                        tmp = null;
                    }
                    return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully, Data = tmp };
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
        public async Task<ResponseModel> inActiveEVoucher(int id)
        {
            try
            {
                TokenManager TKmgr = new TokenManager(_appSettings);
                var context = _httpContextAccessor.HttpContext;
                Token claim = TKmgr.GetClaimToken(context);
                if (!string.IsNullOrEmpty(claim.ph_no) && !string.IsNullOrEmpty(claim.country_code) && claim.user_role == UserRole.Shop.ToString())
                {
                    var voucher_data = _EVoucherRepository.GetByExp(x => x.active_flag == true && x.id == id).FirstOrDefault();
                    if (voucher_data != null)
                    {
                        voucher_data.status = false;
                        var updateData = await _EVoucherRepository.Update(voucher_data);
                        if (updateData)
                        {
                            return new ResponseModel() { Status = APIStatus.Successful, Message = Message.UpdateSucess };
                        }
                        else
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.UpdateFailed };
                        }
                    }
                    else
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.ErrorWhileFetchingData };
                    }
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
        public async Task<bool> ExpiredEVoucher(int id)
        {
            try
            {
                EVoucher eVoucher = _EVoucherRepository.GetByExp(x=>x.id==id).FirstOrDefault();
                if (eVoucher != null && eVoucher.active_flag)
                {
                    eVoucher.active_flag = false;
                    return await _EVoucherRepository.Update(eVoucher);
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
