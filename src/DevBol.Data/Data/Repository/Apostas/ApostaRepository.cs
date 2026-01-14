/*

namespace DevBol.Infrastructure.Data.Repository.Apostas
{
    public class ApostaRepository : Repository<Aposta>, IApostaRepository
    {
        private readonly MeuDbContext _context;
        private readonly ILogger<ApostaRepository> _logger;

        public ApostaRepository(MeuDbContext context,
                                ILogger<ApostaRepository> logger) : base(context)
        {          
            _logger = logger;
        }

        public async Task<Aposta> ObterAposta(Guid id)
        {            

              return await Db.Apostas.AsNoTracking()
              .FirstOrDefaultAsync(r => r.Id == id);
          
        }
               

        public async Task<IEnumerable<Aposta>> ObterApostasDoJogo(Guid jogoId, bool asNoTracking = true)
        {
            IQueryable<Aposta> query = Db.Apostas
                                         .Include(a => a.Jogo)
                                         .Include(a => a.ApostadorCampeonato)
                                         .Where(a => a.JogoId == jogoId)
                                         .OrderBy(a => a.DataHoraAposta);

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }


        public async Task<IEnumerable<Aposta>> ObterApostasDaRodada(Guid rodadaId)
        {
                      return await Db.Apostas.AsNoTracking()
                                   .Include(a => a.Jogo)
                                   .Include(a => a.ApostadorCampeonato)
                                   .Include(a => a.ApostadorCampeonato.Apostador.Usuario)
                                   .Include(a => a.Jogo.EquipeCasa)
                                   .Include(a => a.Jogo.EquipeCasa.Equipe)
                                   .Include(a => a.Jogo.EquipeVisitante)
                                   .Include(a => a.Jogo.EquipeVisitante.Equipe)                                   
                                   .Where(a => a.Jogo.RodadaId == rodadaId)
                                   .OrderBy(a => a.ApostadorCampeonato.Apostador.Usuario.Apelido).ToListAsync();

        }

        public async Task<bool> ExisteApostaNaRodada(Guid rodadaId)
        {
            var aposta = await Db.Apostas.AsNoTracking()
                        .Where(a => a.Jogo.RodadaId == rodadaId)
                        .FirstOrDefaultAsync();

            if (aposta == null)
            {
                return false;
            }
            
            return true;

        }


        public async Task<bool> ExisteApostaSalvaNaRodada(Guid rodadaId)
        {
            var aposta = await Db.Apostas.AsNoTracking()
                        .Where(a => a.Jogo.RodadaId == rodadaId && a.Enviada)
                        .FirstOrDefaultAsync();

            if (aposta == null)
            {
                return false;
            }

            return true;

        }

        public async Task<List<Guid>> ObterApostadoresDaRodada(Guid rodadaId)
        {

            _logger.LogInformation($"Iniciando ObterApostadoresDaRodada para rodada {rodadaId}");

            try
            {
                var apostadores = await Db.Apostas
                 .AsNoTracking()
                 .Where(a => a.Jogo.RodadaId == rodadaId)
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
       

        //Lista Todas as APOSTAS de um APOSTADOR
        public async Task<IEnumerable<Aposta>> ObterApostasDoApostador(Guid apostadorId)
        {
           
                try
                {
                    return await Db.Apostas.AsNoTracking()
                                       .Include(a => a.Jogo)
                                       .Include(a => a.ApostadorCampeonato)
                                       .Where(a => a.ApostadorCampeonatoId == apostadorId)
                                       .OrderBy(a => a.DataHoraAposta).ToListAsync();

                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError($"Erro InvalidOperationException ao obter apostadores do apostador {apostadorId}: {ex.Message}");
                    return (Enumerable.Empty<Aposta>());// Ou lance a exceção novamente, se necessário
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao obter apostas do apostador {apostadorId}: {ex.Message}");
                    return (Enumerable.Empty<Aposta>());// Ou lance a exceção novamente, se necessário
                }
         
        }

        public async Task<IEnumerable<Aposta>> ObterApostasDoApostadorNaRodada(Guid apostadorId, Guid rodadaId)
        {
            
                try
                {
                    return await Db.Apostas.AsNoTracking()
                                 //.Include(a => a.Jogo)
                                 .Where(a => a.Jogo.RodadaId == rodadaId && a.ApostadorCampeonatoId == apostadorId)
                                 .OrderBy(a => a.DataHoraAposta).ToListAsync();
                }
                catch (InvalidOperationException ex)
                {

                    _logger.LogError($"Erro InvalidOperationException ao obter apostadores do apostador {apostadorId} na rodada {rodadaId}: {ex.Message}");
                    return (Enumerable.Empty<Aposta>());// Ou lance a exceção novamente, se necessário
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao obter apostas do apostador {apostadorId} na rodada {rodadaId} : {ex.Message}");
                    return (Enumerable.Empty<Aposta>());// Ou lance a exceção novamente, se necessário
                }
        
        }

        public async Task<Aposta> ObterApostaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorId)
        {
          
                return await Db.Apostas.AsNoTracking()
                        .AsNoTracking()
                        .Where(a => a.ApostadorCampeonato.Id == apostadorId && a.Jogo.Rodada.Id == rodadaId)
                        .FirstOrDefaultAsync();
            
                         
        }

        public async Task<IEnumerable<Aposta>> ObterApostasSalvasNaRodada(Guid rodadaId)
        {

            return await Db.Apostas.AsNoTracking()
                    .AsNoTracking()
                    .Where(a => a.Jogo.Rodada.Id == rodadaId && a.Enviada).ToListAsync();                 


        }

    }

}
*/