using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext dbContext;

        // ASK CLR to create object from StoreContext implicitly
        public GenericRepository(StoreContext dbContext_)
        {
            dbContext = dbContext_;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if(typeof(T) == typeof(Product))
                return (IReadOnlyList<T>) await dbContext.Products.Include(P => P.ProductBrand).Include(P => P.ProductType).ToListAsync();
            return await dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int Id)
        {
            //return await dbContext.Set<T>().Where(X => X.Id == Id).FirstOrDefaultAsync();
            return await dbContext.Set<T>().FindAsync(Id);
        }

        public async Task<T> GetByIdWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>(), spec);
        }

        public async Task<int> GetCountWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public async Task AddAsync(T entity)
            => await dbContext.AddAsync(entity);
        
        public void Update(T entity)
            => dbContext.Update(entity);
        
        public void Delete(T entity)
            => dbContext.Remove(entity);
        
    }
}
