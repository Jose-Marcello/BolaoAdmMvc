using DevBol.Domain.Models.Base;
using DevBol.Infrastructure.Data.Base;
using DevBol.Infrastructure.Data.Context;


namespace DevBol.Application
{
        public interface IUnitOfWork : IDisposable
        {
            //IDbContextTransaction BeginTransaction();
            //void BeginTransaction();
            //void Commit();
            Task<int> SaveChanges();
            void Rollback();
           
        // Adicione métodos para acessar os repositórios
            Task CommitAsync();
            Task RollbackAsync();
           
            int GetTransactionHashCode();
            IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity, new();

            // Adicione esta propriedade para expor o DbContext
            //MeuDbContext DbContext { get; }

            void DetachEntity<TEntity>(TEntity entity) where TEntity : class;


    }
    }

