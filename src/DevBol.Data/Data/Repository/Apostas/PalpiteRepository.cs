using Microsoft.EntityFrameworkCore; // Para Include, Where, ExecuteDeleteAsync, AsNoTracking, ToListAsync
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq; // Para Where, Any()
using System.Threading.Tasks;
using DevBol.Domain.Models.Apostas;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Context;
// REMOVER ESTA LINHA SE AINDA EXISTIR: using System.Data.Entity;


namespace ApostasApp.Core.Infrastructure.Data.Repository.Apostas
{
    public class PalpiteRepository : Repository<Palpite>, IPalpiteRepository
    {
        private readonly ILogger<PalpiteRepository> _logger;
        public PalpiteRepository(MeuDbContext context, ILogger<PalpiteRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Palpite>> ObterPalpitesDaRodada(Guid rodadaId)
        {
            return await DbSet.AsNoTracking()
                              .Include(p => p.Jogo) // Incluir dados do Jogo
                                  .ThenInclude(j => j.EquipeCasa) // EquipeCampeonato
                                      .ThenInclude(ec => ec.Equipe) // Equipe (nome, escudo, sigla)
                              .Include(p => p.Jogo)
                                  .ThenInclude(j => j.EquipeVisitante) // EquipeCampeonato
                                      .ThenInclude(ev => ev.Equipe) // Equipe (nome, escudo, sigla)
                                                                    // <<-- CORREÇÃO AQUI: Navega de Palpite -> ApostaRodada -> ApostadorCampeonato (a adesão) -> Apostador (o apostador real) -> Usuario
                              .Include(p => p.ApostaRodada) // 'p' é Palpite, tem ApostaRodada
                                  .ThenInclude(ar => ar.ApostadorCampeonato) // 'ar' é ApostaRodada, tem ApostadorCampeonato (a adesão)
                                      .ThenInclude(ac => ac.Apostador) // 'ac' é ApostadorCampeonato, tem Apostador (o apostador real)
                                          .ThenInclude(apostador => apostador.Usuario) // 'apostador' é do tipo Apostador, que tem a propriedade Usuario.
                              .Where(p => p.Jogo.RodadaId == rodadaId)
                              .ToListAsync();
        }


        public async Task<IEnumerable<Palpite>> ObterPalpitesDoApostadorNaRodada(Guid rodadaId, Guid ApostadorCampeonatoId)
        {
            return await DbSet.AsNoTracking()
                              .Include(p => p.Jogo) // Incluir dados do Jogo
                                  .ThenInclude(j => j.EquipeCasa) // EquipeCampeonato
                                      .ThenInclude(ec => ec.Equipe) // Equipe (nome, escudo, sigla)
                              .Include(p => p.Jogo)
                                  .ThenInclude(j => j.EquipeVisitante) // EquipeCampeonato
                                      .ThenInclude(ev => ev.Equipe) // Equipe (nome, escudo, sigla)
                                                                    // <<-- CORREÇÃO AQUI: Navega de Palpite -> ApostaRodada -> ApostadorCampeonato (a adesão) -> Apostador (o apostador real) -> Usuario
                              .Include(p => p.ApostaRodada) // 'p' é Palpite, tem ApostaRodada
                                  .ThenInclude(ar => ar.ApostadorCampeonato) // 'ar' é ApostaRodada, tem ApostadorCampeonato (a adesão)
                                      .ThenInclude(ac => ac.Apostador) // 'ac' é ApostadorCampeonato, tem Apostador (o apostador real)
                                          .ThenInclude(apostador => apostador.Usuario) // 'apostador' é do tipo Apostador, que tem a propriedade Usuario.
                              .Where(p => p.Jogo.RodadaId == rodadaId && p.ApostaRodada.ApostadorCampeonatoId == ApostadorCampeonatoId)
                              .ToListAsync();
        }



        public async Task<int> ObterTotaldePontosdoApostadorNaRodada(Guid rodadaId, Guid ApostadorCampeonatoId)
        {
            // O método agora retorna a soma total da pontuação
            return await DbSet.AsNoTracking()
                .Where(p => p.Jogo.RodadaId == rodadaId && p.ApostaRodada.ApostadorCampeonatoId == ApostadorCampeonatoId)
                .SumAsync(p => p.Pontos); // Soma a pontuação de todos os palpites encontrados
        }


        /// <summary>
        /// Remove todos os palpites associados a uma rodada específica de forma eficiente.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada cujos palpites serão removidos.</param>
        /// <returns>True se a operação for bem-sucedida, false caso contrário.</returns>
        public async Task<bool> RemoverTodosPalpitesDaRodada(Guid rodadaId)
        {
            try
            {
                var affectedRows = await DbSet.Where(p => p.Jogo.RodadaId == rodadaId)
                                              .ExecuteDeleteAsync(); // Método eficiente para deleção em massa

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover palpites da rodada {RodadaId}", rodadaId);
                return false;
            }

        }

        public async Task AdicionarRange(IEnumerable<Palpite> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }





    }
}
