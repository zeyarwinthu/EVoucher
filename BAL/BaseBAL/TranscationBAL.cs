using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Model;
using WebAPI.Model.Entity;
using WebAPI.Model.Request;
using WebAPI.Repository;

namespace WebAPI.BAL
{
    public interface ITranscationBAL : IGenericBAL<Transcation>
    {
        Task<bool> CreateTranscation(TranscationRequest inputModel);
    }
    public class TranscationBAL : GenericBAL<Transcation>, ITranscationBAL
    {
        private readonly Appsetting _appSettings;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Transcation> _TranscationRepository;
        public TranscationBAL(IGenericRepository<Transcation> TranscationRepository, IOptions<Appsetting> appsettings, IMapper mapper) : base(TranscationRepository)
        {
            _TranscationRepository = TranscationRepository;
            _mapper = mapper;
            _appSettings = appsettings.Value;
        }
        public async Task<bool> CreateTranscation(TranscationRequest inputModel)
        {
            try
            {
                Transcation transcation = new Transcation()
                {
                    evoucher_id = inputModel.evoucher_id,
                    payment_method_id = inputModel.payment_method_id,
                    amount = inputModel.amount,
                    user_country_code = inputModel.user_country_code,
                    user_ph_no = inputModel.user_ph_no,
                    active_flag = true,
                    created_date=DateTime.UtcNow
                };
                bool obj =await _TranscationRepository.Create(transcation);
                return obj;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
