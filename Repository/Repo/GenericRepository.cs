using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Model;

namespace WebAPI.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetByExp(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetByExp(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, string>> predicateOrder);
        Task<bool> Create(TEntity entity);
        Task<bool> CreateRange(List<TEntity> entitylist);
        Task<bool> Update(TEntity entity);
        Task<bool> Delete(TEntity entity);
    }
    public class GenericRepository<TEntity>: IGenericRepository<TEntity> where TEntity :class
    {
        private readonly DB_Context _dbContext;
        public GenericRepository(DB_Context dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Create(TEntity entity)
        {
            try
            {
                await _dbContext.Set<TEntity>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> Update(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Update(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> Delete(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Remove(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateRange(List<TEntity> entityList)
        {
            try
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                await _dbContext.Set<TEntity>().AddRangeAsync(entityList);
                _dbContext.ChangeTracker.DetectChanges();
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IQueryable<TEntity> GetByExp(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return _dbContext.Set<TEntity>().Where(predicate).AsNoTracking();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IQueryable<TEntity> GetByExp(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, string>> predicateOrder)
        {
            try
            {
                return _dbContext.Set<TEntity>().Where(predicate).AsNoTracking();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
