using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.IRepositories;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext dbContext;
        private Hashtable repositories;

        public UnitOfWork(StoreContext dbContext)
        {
            this.dbContext = dbContext;
            repositories = new Hashtable();
        }

        public IGenericRepository<TEntity>? Repository<TEntity>() where TEntity : BaseEntity
        {
            //if (repositories is null)
            //    repositories = new Hashtable();
            
            var type = typeof(TEntity).Name;

            if (!repositories.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(dbContext);
                repositories.Add(type, repository);
            }

            return repositories[type] as IGenericRepository<TEntity>;
        }

        public async Task<int> Complete()
            => await dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await dbContext.DisposeAsync();

        
    }
}
