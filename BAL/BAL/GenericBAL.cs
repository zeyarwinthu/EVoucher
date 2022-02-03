using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Repository;

namespace WebAPI.BAL
{
    public interface IGenericBAL<TEntity>
      where TEntity : class
    {
        List<TEntity> GetByExp(Expression<Func<TEntity, bool>> predicate);
        Task<bool> Update(TEntity entity);
        Task<bool> Create(TEntity entity);
        Task<bool> CreateRange(List<TEntity> entitylist);
        Task<bool> Delete(TEntity entity);
    }
    public class GenericBAL<TEntity> : IGenericBAL<TEntity>
       where TEntity : class
    {
        private readonly IGenericRepository<TEntity> _GenericRepository;

        public GenericBAL(IGenericRepository<TEntity> GenericRepository)
        {
            _GenericRepository = GenericRepository;
        }

        public async Task<bool> Create(TEntity entity)
        {
            try
            {
                return await _GenericRepository.Create(entity);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateRange(List<TEntity> entity)
        {
            try
            {
                return await _GenericRepository.CreateRange(entity);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> Delete(TEntity entity)
        {
            return await _GenericRepository.Delete(entity);
        }

        public List<TEntity> GetByExp(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return new List<TEntity>(_GenericRepository.GetByExp(predicate));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> Update(TEntity entity)
        {
            try
            {
                var obj = await _GenericRepository.Update(entity);
                if (obj)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
