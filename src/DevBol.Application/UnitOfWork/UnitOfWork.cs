using DevBol.Domain.Models.Apostadores;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Base;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Equipes;
using DevBol.Domain.Models.Estadios;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.RankingRodadas;
using DevBol.Domain.Models.Rodadas;
using DevBol.Domain.Models.Ufs;
using DevBol.Domain.Models.Usuarios;
using DevBol.Infrastructure.Data.Base;
using DevBol.Infrastructure.Data.Context;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Infrastructure.Data.Repository.Apostadores;
using DevBol.Infrastructure.Data.Repository.Apostas;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Equipes;
using DevBol.Infrastructure.Data.Repository.Estadios;
using DevBol.Infrastructure.Data.Repository.Jogos;
using DevBol.Infrastructure.Data.Repository.RankingRodadas;
using DevBol.Infrastructure.Data.Repository.Rodadas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace DevBol.Application
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeuDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories;
        private readonly ILoggerFactory _loggerFactory; // Adicione ILoggerFactory

        public UnitOfWork(MeuDbContext context,
                          UserManager<Usuario> userManager,
                          ILoggerFactory loggerFactory) // Receba ILoggerFactory
        {
            _context = context;
            _userManager = userManager;
            _repositories = new Dictionary<Type, object>();
            _loggerFactory = loggerFactory;
            //_transaction = _context.Database.BeginTransaction();
        }

        // Implemente a propriedade DbContext
        //public MeuDbContext DbContext => _context;

       /* public void BeginTransaction()
        {
            // Transação já iniciada no construtor
            Debug.WriteLine($"Transaction iniciada (já estava ativa): {_transaction?.GetHashCode() ?? 0}");
        }
*/
        public int GetTransactionHashCode()
        {
            return _transaction?.GetHashCode() ?? 0;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity, new()
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return (IRepository<TEntity>)_repositories[typeof(TEntity)];
            }

            object repositoryInstance = null;

            if (typeof(TEntity) == typeof(JogoFinalizadoEvent))
            {
                var logger = _loggerFactory.CreateLogger<ApostadorRepository>();
                repositoryInstance = new ApostadorRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(ApostaRodada))
            {
                var logger = _loggerFactory.CreateLogger<ApostaRodadaRepository>();
                repositoryInstance = new ApostaRodadaRepository(_context, logger);
            }
            //else if (typeof(TEntity) == typeof(Palpite))
            //{
            //    var logger = _loggerFactory.CreateLogger<Palpite>();
            //    repositoryInstance = new PalpiteRepository(_context, logger);
            //}
            else if (typeof(TEntity) == typeof(ApostadorCampeonato))
            {
                var logger = _loggerFactory.CreateLogger<ApostadorCampeonatoRepository>();
                repositoryInstance = new ApostadorCampeonatoRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(EquipeCampeonato))
            {
                var logger = _loggerFactory.CreateLogger<EquipeCampeonatoRepository>();
                repositoryInstance = new EquipeCampeonatoRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Equipe))
            {
                var logger = _loggerFactory.CreateLogger<EquipeRepository>();
                repositoryInstance = new EquipeRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Campeonato))
            {
                var logger = _loggerFactory.CreateLogger<CampeonatoRepository>();
                repositoryInstance = new CampeonatoRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Estadio))
            {
                var logger = _loggerFactory.CreateLogger<EstadioRepository>();
                repositoryInstance = new EstadioRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Jogo))
            {
                var logger = _loggerFactory.CreateLogger<JogoRepository>();
                repositoryInstance = new JogoRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(RankingRodada))
            {
                var logger = _loggerFactory.CreateLogger<RankingRodadaRepository>();
                repositoryInstance = new RankingRodadaRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Rodada))
            {
                var logger = _loggerFactory.CreateLogger<RodadaRepository>();
                repositoryInstance = new RodadaRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Uf))
            {
                var logger = _loggerFactory.CreateLogger<UfRepository>();
                repositoryInstance = new UfRepository(_context, logger);
            }
            else
            {
                return null;
            }

            _repositories[typeof(TEntity)] = repositoryInstance;
            return (IRepository<TEntity>)repositoryInstance;
        }                        
             

        public async Task<int> SaveChanges()
        {
            var strategy = _context.Database.CreateExecutionStrategy(); // Obtém a estratégia de repetição

            return await strategy.ExecuteAsync(async () => // Executa SaveChanges dentro da estratégia
            {
                using (var transaction = _context.Database.BeginTransaction()) // Inicia a transação AQUI
                {
                    try
                    {
                        int result = await _context.SaveChangesAsync();
                        transaction.Commit();
                        return result; // Retorna o resultado de SaveChangesAsync
                    }
                    catch (DbUpdateException ex)
                    {
                        _transaction?.Rollback(); // Rollback da transação
                        throw new Exception("Erro ao salvar alterações no banco de dados.", ex);
                    }
                    catch (ObjectDisposedException ex)
                    {
                        _transaction?.Rollback();
                        throw new Exception("Erro ao salvar alterações devido a um objeto descartado.", ex);
                    }
                    catch (Exception ex)
                    {
                        _transaction?.Rollback();
                        throw new Exception("Erro ao salvar alterações.", ex);
                    }
                    finally
                    {
                        transaction?.Dispose(); // Garante que a transação seja descartada
                    }
                }
            });
        }


       /* public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _transaction.Commit();
                Debug.WriteLine($"Transaction COMMIT: {_transaction.GetHashCode()}");
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine($"Erro ao salvar alterações (DbUpdateException): {ex.Message}");
                Debug.WriteLine(ex);
                _transaction.Rollback();
                throw new Exception("Erro ao salvar alterações no banco de dados.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine($"Erro ao salvar alterações (ObjectDisposedException): {ex.Message}");
                Debug.WriteLine(ex);
                _transaction.Rollback();
                throw new Exception("Erro ao salvar alterações devido a um objeto descartado.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar alterações: {ex.Message}");
                Debug.WriteLine(ex);
                _transaction.Rollback();
                throw new Exception("Erro ao salvar alterações.", ex);
            }
        }
*/
        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void DetachEntity<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                _transaction?.Commit();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                await Task.Yield(); // Para tornar o método assíncrono
                _transaction?.Rollback();
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

    }
}