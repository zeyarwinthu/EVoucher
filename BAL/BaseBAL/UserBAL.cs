using AutoMapper;
using Hangfire;
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
    public interface IUserBAL : IGenericBAL<User>
    {
        Task<ResponseModel> createUser(UserRequest inputModel, UserRole role);
        Task<ResponseModel> shopRequestOTP(string country_code, string ph_no);
        Task<ResponseModel> shopVerifyOTP(string country_code, string ph_no, string otp);
        Task<ResponseModel> customerRequestOTP(string country_code, string ph_no);
        Task<ResponseModel> customerVerifyOTP(string country_code, string ph_no, string otp);
    }
    public class UserBAL : GenericBAL<User>, IUserBAL
    {
        private readonly Appsetting _appSettings;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<User> _UserRepository;
        public UserBAL(IGenericRepository<User> UserRepository, IOptions<Appsetting> appsettings, IMapper mapper) : base(UserRepository)
        {
            _UserRepository = UserRepository;
            _mapper = mapper;
            _appSettings = appsettings.Value;
        }
        public async Task<ResponseModel> createUser(UserRequest inputModel,UserRole role)
        {
            try
            {
                var check_ph = checkPhone(inputModel.country_code, inputModel.ph_no);
                if (check_ph)
                {
                    return new ResponseModel() { Status = APIStatus.Error, Message = Message.phExist };
                }
                else
                {
                    var obj = _mapper.Map<User>(inputModel);
                    obj.id = Guid.NewGuid();
                    obj.role = role.ToString();
                    obj.created_date = DateTime.UtcNow;
                    obj.active_flag = true;
                    var create_User = await _UserRepository.Create(obj);
                    if (create_User)
                    {
                        return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully, Data = new UserResponse() {
                            id = obj.id,
                            first_name = obj.first_name,
                            last_name = obj.last_name,
                            country_code=obj.country_code,
                            ph_no = obj.ph_no,
                            role = obj.role,
                        } };
                    }
                    else
                    {
                        return new ResponseModel(){Status = APIStatus.Error,Message = Message.AddFailed };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }
        public async Task<ResponseModel> shopRequestOTP(string country_code,string ph_no)
        {
            try
            {
                Random random = new Random();
                const string number = "0123456789";
                var otp = new string(Enumerable.Repeat(number, 6).Select(s => s[random.Next(s.Length)]).ToArray());
                var user_data = _UserRepository.GetByExp(x => x.country_code == country_code && x.ph_no == ph_no).FirstOrDefault();
                if (user_data != null)
                {
                    if (user_data.active_flag == false)
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.contectAdmin };
                    }
                    else if (user_data.role != UserRole.Shop.ToString())
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.notAdmin };
                    }
                    else
                    {
                        user_data.otp = otp;
                        var upd_user =await _UserRepository.Update(user_data);
                        if (upd_user)
                        {
                            BackgroundJob.Schedule(() => clearOTP(user_data.id), DateTime.UtcNow.AddMinutes(5));
                            return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully, Data=new { otp=user_data.otp} };
                        }
                        else
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.UpdateFailed };
                        }
                    }
                }
                return new ResponseModel() { Status = APIStatus.Error, Message = Message.notRegister };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }
        public async Task<ResponseModel> shopVerifyOTP(string country_code,string ph_no,string otp)
        {
            try
            {
                var user_data = _UserRepository.GetByExp(x => x.country_code == country_code && x.ph_no == ph_no).FirstOrDefault();
                if (user_data != null)
                {
                    if (user_data.active_flag == false)
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.contectAdmin };
                    }
                    else if (user_data.role != UserRole.Shop.ToString())
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.notAdmin };
                    }
                    else
                    {
                        if (user_data.otp == otp)
                        {
                            TokenManager TKmgr = new TokenManager(_appSettings);
                            var returnData = new UserResponse()
                            {
                                id = user_data.id,
                                first_name = user_data.first_name,
                                last_name = user_data.last_name,
                                ph_no = user_data.ph_no,
                                country_code=user_data.country_code,
                                role = user_data.role,
                            };
                            var token = TKmgr.GenToken(returnData);
                            returnData.Token = token;
                            await clearOTP(user_data.id);
                            return new ResponseModel()
                            {
                                Status = APIStatus.Successful,
                                Message = Message.Successfully,
                                Data = returnData
                            };
                        }
                        else
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.wrongOTP };
                        }
                    }
                }
                return new ResponseModel() { Status = APIStatus.Error, Message = Message.notRegister };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }
        public async Task<ResponseModel> customerRequestOTP(string country_code,string ph_no)
        {
            try
            {
                Random random = new Random();
                const string number = "0123456789";
                var otp = new string(Enumerable.Repeat(number, 6).Select(s => s[random.Next(s.Length)]).ToArray());
                var user_data = _UserRepository.GetByExp(x => x.country_code == country_code && x.ph_no == ph_no).FirstOrDefault();
                if (user_data != null)
                {
                    if (user_data.active_flag == false)
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.contectAdmin };
                    }
                    else if (user_data.role != UserRole.User.ToString())
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.notCustomer };
                    }
                    else
                    {
                        user_data.otp = otp;
                        var upd_user =await _UserRepository.Update(user_data);
                        if (upd_user)
                        {
                            BackgroundJob.Schedule(() => clearOTP(user_data.id), DateTime.UtcNow.AddMinutes(5));
                            return new ResponseModel() { Status = APIStatus.Successful, Message = Message.Successfully, Data=new { otp=user_data.otp} };
                        }
                        else
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.UpdateFailed };
                        }
                    }
                }
                return new ResponseModel() { Status = APIStatus.Error, Message = Message.notRegister };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }
        public async Task<ResponseModel> customerVerifyOTP(string country_code,string ph_no,string otp)
        {
            try
            {
                var user_data = _UserRepository.GetByExp(x => x.country_code == country_code && x.ph_no == ph_no).FirstOrDefault();
                if (user_data != null)
                {
                    if (user_data.active_flag == false)
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.contectAdmin };
                    }
                    else if (user_data.role != UserRole.User.ToString())
                    {
                        return new ResponseModel() { Status = APIStatus.Error, Message = Message.notCustomer };
                    }
                    else
                    {
                        if (user_data.otp == otp)
                        {
                            TokenManager TKmgr = new TokenManager(_appSettings);
                            var returnData = new UserResponse()
                            {
                                id = user_data.id,
                                first_name = user_data.first_name,
                                last_name = user_data.last_name,
                                ph_no = user_data.ph_no,
                                country_code=user_data.country_code,
                                role = user_data.role,
                            };
                            var token = TKmgr.GenToken(returnData);
                            returnData.Token = token;
                            await clearOTP(user_data.id);
                            return new ResponseModel()
                            {
                                Status = APIStatus.Successful,
                                Message = Message.Successfully,
                                Data = returnData
                            };
                        }
                        else
                        {
                            return new ResponseModel() { Status = APIStatus.Error, Message = Message.wrongOTP };
                        }
                    }
                }
                return new ResponseModel() { Status = APIStatus.Error, Message = Message.notRegister };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Status = APIStatus.SystemError, Message = Message.c_SystemError };
            }
        }
        public bool checkPhone(string country_code,string ph_no)
        {
            try
            {
                var ph_data = _UserRepository.GetByExp(x => x.country_code == country_code && x.ph_no == ph_no).FirstOrDefault();
                if (ph_data != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> clearOTP(Guid id)
        {
            try
            {
                var user_data = _UserRepository.GetByExp(x => x.id == id && x.active_flag==true).FirstOrDefault();
                if (user_data != null && !string.IsNullOrEmpty(user_data.otp))
                {
                    user_data.otp = "";
                    return await _UserRepository.Update(user_data);
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
