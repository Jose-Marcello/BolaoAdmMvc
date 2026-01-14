using DevBol.Infrastructure.Data.Context;

namespace DevBol.Infrastructure.Data.Interfaces.Base
{
    public interface IDbContextFactory
    {
        MeuDbContext CriarContexto();
    }

}