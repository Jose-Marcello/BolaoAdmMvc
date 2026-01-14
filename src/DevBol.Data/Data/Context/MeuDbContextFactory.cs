using DevBol.Infrastructure.Data.Context;
using DevBol.Infrastructure.Data.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Infrastructure;

namespace DevBol.Infrastructure.Data.Context
{
    public class MeuDbContextFactory : IDbContextFactory
    {
        private readonly DbContextOptions<MeuDbContext> _options;

        public MeuDbContextFactory(DbContextOptions<MeuDbContext> options)
        {
            _options = options;
        }

        public MeuDbContext CriarContexto()
        {
            return new MeuDbContext(_options);
        }
    }

}