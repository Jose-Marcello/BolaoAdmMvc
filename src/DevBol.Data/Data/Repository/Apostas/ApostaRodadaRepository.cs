using DevBol.Domain.Models.Apostas;
using DevBol.Infrastructure.Data.Context;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevBol.Infrastructure.Data.Repository.Apostas
{

    public class ApostaRodadaRepository : Repository<ApostaRodada>, IApostaRodadaRepository
    {
        private readonly ILogger<ApostaRodadaRepository> _logger;

        // Sobrecarga de construtor sem ILogger, se ainda usar. Idealmente, ter apenas um.
        public ApostaRodadaRepository(MeuDbContext context, ILogger<ApostaRodadaRepository> logger) : base(context)
        {
            _logger = logger;
        }


        // Se este construtor é usado em algum lugar (ex: testes simples), mantenha.
        // Se não, remova-o e injete o logger sempre.
        public ApostaRodadaRepository(MeuDbContext db) : base(db)
        {
            // _logger = null; // Cuidado com NullReferenceException se usar _logger
        }


        public async Task<ApostaRodada> ObterStatusApostaRodada(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            return await Db.ApostasRodada // Agora busca na coleção de ApostaRodada
                           .Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId && ar.RodadaId == rodadaId)
                           .FirstOrDefaultAsync();

        }

        public async Task<ApostaRodada> ObterApostaRodadaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            return await Db.ApostasRodada // Agora busca na coleção de ApostaRodada
                           .Include(ar => ar.Palpites) // Se precisar dos palpites para algo mais (não para o status de envio aqui)
                           .AsNoTracking()
                           .Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId && ar.RodadaId == rodadaId)
                           .FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<ApostaRodada>> ObterApostasRodadaApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            return await Db.ApostasRodada // Agora busca na coleção de ApostaRodada
                                          //.Include(ar => ar.Palpites) // Se precisar dos palpites para algo mais (não para o status de envio aqui)
                           .AsNoTracking()
                           .Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId && ar.RodadaId == rodadaId).ToListAsync();

        }

        public async Task<IEnumerable<ApostaRodada>> ObterApostasDaRodada(Guid rodadaId)
        {
            return await Db.ApostasRodada.AsNoTracking()
                         .Include(ar => ar.ApostadorCampeonato)
                            .ThenInclude(ac => ac.Apostador)
                               .ThenInclude(ap => ap.Usuario)
                         .Include(ar => ar.Rodada)
                         .Where(ar => ar.RodadaId == rodadaId)
                         .OrderBy(ar => ar.ApostadorCampeonato.Apostador.Usuario.Apelido).ToListAsync();

        }


        //indica que já existe alguma aposta na Rodada (usado p/ não gerar apostas repetidas)
        public async Task<bool> ExisteApostaNaRodada(Guid rodadaId)
        {
            var aposta = await Db.ApostasRodada.AsNoTracking()
                        .Where(a => a.RodadaId == rodadaId)
                        .FirstOrDefaultAsync();

            if (aposta == null)
            {
                return false;
            }

            return true;

        }


        public async Task<bool> ExisteApostaRodadaSalvaNaRodada(Guid rodadaId)
        {
            var apostaRodada = await Db.ApostasRodada.AsNoTracking()
                        .Where(a => a.Rodada.Id == rodadaId && a.Enviada)
                        .FirstOrDefaultAsync();

            if (apostaRodada == null)
            {
                return false;
            }

            return true;

        }



        // Exemplo de implementação no ApostaRodadaRepository
        public async Task<ApostaRodada> ObterUltimaApostaRodadaDoApostadorNaRodada(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            // Esta lógica depende de como você define "última" ou "ativa".
            // Opção 1: A aposta que ainda está em edição (DataHoraSubmissao é null)
            var apostaEmEdicao = await Db.ApostasRodada.Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId &&
                                                          ar.RodadaId == rodadaId &&
                                                          !ar.DataHoraSubmissao.HasValue)
                                            .FirstOrDefaultAsync();

            //if (apostaEmEdicao != null)
            //{
            return apostaEmEdicao;
            //}

            // Opção 2: Se não há aposta em edição, pega a última submetida
            // (Isso será relevante quando o usuário puder ter múltiplas ApostaRodadas submetidas)
            //return await DbSet.Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId &&
            //                                ar.RodadaId == rodadaId &&
            //                               ar.DataHoraSubmissao.HasValue)
            //                 .OrderByDescending(ar => ar.DataHoraSubmissao)
            //                 .FirstOrDefaultAsync();
        }


        //indica se um ApostadorCampeonato (apostador registrado que já jogou em algum momento)
        //ADERIU AO CAMPEONATO SELECIONADO
        public async Task<bool> EAderido(Guid apostadorCampeonatoId)
        {
            var aposta = await Db.ApostasRodada.AsNoTracking()
                        .Where(a => a.ApostadorCampeonatoId == apostadorCampeonatoId && a.EhApostaCampeonato)
                        .FirstOrDefaultAsync();

            if (aposta == null)
            {
                return false;
            }

            return true;

        }
    

    //Lista Todas as APOSTAS de um APOSTADOR
        public async Task<IEnumerable<ApostaRodada>> ObterApostasDoApostador(Guid apostadorCampeonatoId)
        {

            try
            {
                return await Db.ApostasRodada.AsNoTracking()
                                   .Include(a => a.ApostadorCampeonato)
                                   .Where(a => a.ApostadorCampeonatoId == apostadorCampeonatoId)
                                   .OrderBy(a => a.DataHoraSubmissao).ToListAsync();

            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Erro InvalidOperationException ao obter apostadores do apostador {apostadorCampeonatoId}: {ex.Message}");
                return (Enumerable.Empty<ApostaRodada>());// Ou lance a exceção novamente, se necessário
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter apostas do apostador {apostadorCampeonatoId}: {ex.Message}");
                return (Enumerable.Empty<ApostaRodada>());// Ou lance a exceção novamente, se necessário
            }

        }

        public async Task<IEnumerable<Palpite>> ObterPalpitesDoJogo(Guid jogoId, bool asNoTracking = true)
        {
            IQueryable<Palpite> query = Db.Palpites
                                         //.Include(a => a.Jogo)
                                         .Include(a => a.ApostaRodada.ApostadorCampeonato)
                                         .Where(a => a.JogoId == jogoId)
                                         .OrderBy(a => a.ApostaRodada.DataHoraSubmissao);

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task<List<Guid>> ObterApostadoresDaRodada(Guid rodadaId)
        {

            _logger.LogInformation($"Iniciando ObterApostadoresDaRodada para rodada {rodadaId}");

            try
            {
                var apostadores = await Db.ApostasRodada
                 .AsNoTracking()
                 .Where(a => a.RodadaId == rodadaId)
                 .Select(a => a.ApostadorCampeonatoId)
                 .Distinct()
                 .ToListAsync();

                _logger.LogInformation($"ObterApostadoresDaRodada concluído com sucesso para rodada {rodadaId}, {apostadores.Count} apostadores encontrados.");

                return apostadores;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Erro InvalidOperationException ao obter apostadores da rodada {rodadaId}: {ex.Message}");
                return new List<Guid>(); // Ou lance a exceção novamente, se necessário
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter apostadores da rodada {rodadaId}: {ex.Message}");
                return new List<Guid>(); // Ou lance a exceção novamente, se necessário
            }
        }

        public async Task<IEnumerable<ApostaRodada>> ObterApostasDoApostadorNaRodada(Guid apostadorId, Guid rodadaId)
        {
            try
            {
                return await Db.ApostasRodada.AsNoTracking()
                             //.Include(a => a.Jogo)
                             .Where(a => a.RodadaId == rodadaId && a.ApostadorCampeonatoId == apostadorId)
                             .OrderBy(a => a.DataHoraSubmissao).ToListAsync();
            }
            catch (InvalidOperationException ex)
            {

                _logger.LogError($"Erro InvalidOperationException ao obter apostadores do apostador {apostadorId} na rodada {rodadaId}: {ex.Message}");
                return (Enumerable.Empty<ApostaRodada>());// Ou lance a exceção novamente, se necessário
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter apostas do apostador {apostadorId} na rodada {rodadaId} : {ex.Message}");
                return (Enumerable.Empty<ApostaRodada>());// Ou lance a exceção novamente, se necessário
            }

        }

        public async Task<ApostaRodada> ObterApostaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorId)
        {

            return await Db.ApostasRodada.AsNoTracking()
                    .AsNoTracking()
                    .Where(a => a.ApostadorCampeonato.Id == apostadorId && a.Rodada.Id == rodadaId)
                    .FirstOrDefaultAsync();


        }

    }
}
