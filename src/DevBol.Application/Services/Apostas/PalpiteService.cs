// Localização: ApostasApp.Core.Application.Services/Palpites/PalpiteService.cs

 
using AutoMapper;
using DevBol.Application.Base;
using DevBol.Application.Interfaces.Apostas;
using DevBol.Domain.Interfaces;
using DevBol.Domain.Models.Apostas;
using DevBol.Infrastructure.Data.Interfaces.Apostas;

using Microsoft.Extensions.Logging;

namespace DevBol.Application.Services.Palpites
{
    public class PalpiteService : BaseService, IPalpiteService
    {
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly IMapper _mapper;
       

        public PalpiteService(
            IApostaRodadaRepository apostaRodadaRepository,
            IPalpiteRepository palpiteRepository,
            IMapper mapper,
            INotificador notificador,
            IUnitOfWork uow)
            
            : base(notificador, uow)
        {
            _apostaRodadaRepository = apostaRodadaRepository;
            _palpiteRepository = palpiteRepository;            
            _mapper = mapper;            
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task AdicionarPalpite(Palpite palpite)
        {
            if (!ExecutarValidacao(new PalpiteValidation(), palpite))
                return;

            await _palpiteRepository.Adicionar(palpite);
        }


        public async Task AtualizarPalpite(Palpite palpite)
        {
            if (!ExecutarValidacao(new PalpiteValidation(), palpite)) return;

            await _palpiteRepository.Atualizar(palpite);
            
            //return;

        }
              

        /// <summary>
        /// Obtém uma coleção de palpites para uma rodada específica.
        /// </summary>
        public async Task<int> ObterTotaldePontosDoApostadornaRodada(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            return await _palpiteRepository.ObterTotaldePontosdoApostadorNaRodada(rodadaId, apostadorCampeonatoId);
        }


    }
}

        /*

        /// <summary>
        /// Obtém uma coleção de palpites para uma rodada específica.
        /// </summary>
        public async Task<IEnumerable<Palpite>> ObterPalpitesDaRodada(Guid rodadaId)
        {
            try
            {
                var palpites = await _palpiteRepository.ObterPalpitesDaRodada(rodadaId);
                
                if (!palpites.Any())
                {
                    //Notificar("Alerta", "Nenhum palpite encontrado para a rodada especificada.");
                    //return new ApiResponse<IEnumerable<PalpiteDto>>(true, "Nenhum palpite encontrado.", new List<PalpiteDto>(), ObterNotificacoesParaResposta().ToList());
                    Notificar("Nenhum palpite encontrado !!");
                    return;
                }

                var palpitesDto = _mapper.Map<IEnumerable<PalpiteDto>>(palpites);
                //return new ApiResponse<IEnumerable<PalpiteDto>>(true, "Palpites obtidos com sucesso.", palpitesDto, ObterNotificacoesParaResposta().ToList());
                Notificar("Palpites obtidos com sucesso !!");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter palpites da rodada.");
                //Notificar("Erro", $"Erro ao obter palpites da rodada: {ex.Message}");
                Notificar("Erro ao obter palpites da rodada: {ex.Message}");
                return new IEnumerable<Palpite>(false, "Ocorreu um erro ao obter os palpites da rodada.", null, ObterNotificacoesParaResposta().ToList());
            }
        }

        public Task IEnumerable<PalpiteDto>(ObterPalpitesDaRodada 1, (Guid rodadaId, object) 2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adiciona um novo palpite.
        /// </summary>
        /// 
        /*
        public async Task<ApiResponse<PalpiteDto>> AdicionarPalpite(SalvarPalpiteRequestDto palpiteRequest)
        {
            try
            {
                // Mapear DTO para entidade
                var palpite = _mapper.Map<Palpite>(palpiteRequest);

                // Adicionar ao repositório
                await _palpiteRepository.Adicionar(palpite);

                // Salvar mudanças
                if (!await CommitAsync())
                {
                    return new ApiResponse<PalpiteDto>(false, "Falha ao adicionar palpite.", null, ObterNotificacoesParaResposta().ToList());
                }

                var palpiteDto = _mapper.Map<PalpiteDto>(palpite);
                return new ApiResponse<PalpiteDto>(true, "Palpite adicionado com sucesso.", palpiteDto, ObterNotificacoesParaResposta().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar palpite.");
                Notificar("Erro", $"Erro ao adicionar palpite: {ex.Message}");
                return new ApiResponse<PalpiteDto>(false, "Ocorreu um erro ao adicionar o palpite.", null, ObterNotificacoesParaResposta().ToList());
            }
        }

        /// <summary>
        /// Atualiza um palpite existente.
        /// </summary>
        public async Task<ApiResponse<PalpiteDto>> AtualizarPalpite(Guid palpiteId, SalvarPalpiteRequestDto palpiteRequest)
        {
            try
            {
                var palpiteExistente = await _palpiteRepository.ObterPorId(palpiteId);
                if (palpiteExistente == null)
                {
                    Notificar("Alerta", "Palpite não encontrado para atualização.");
                    return new ApiResponse<PalpiteDto>(false, "Palpite não encontrado.", null, ObterNotificacoesParaResposta().ToList());
                }

                // Atualizar propriedades do palpite existente
                _mapper.Map(palpiteRequest, palpiteExistente);

                await _palpiteRepository.Atualizar(palpiteExistente);

                if (!await CommitAsync())
                {
                    return new ApiResponse<PalpiteDto>(false, "Falha ao atualizar palpite.", null, ObterNotificacoesParaResposta().ToList());
                }

                var palpiteDto = _mapper.Map<PalpiteDto>(palpiteExistente);
                return new ApiResponse<PalpiteDto>(true, "Palpite atualizado com sucesso.", palpiteDto, ObterNotificacoesParaResposta().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar palpite.");
                Notificar("Erro", $"Erro ao atualizar palpite: {ex.Message}");
                return new ApiResponse<PalpiteDto>(false, "Ocorreu um erro ao atualizar o palpite.", null, ObterNotificacoesParaResposta().ToList());
            }
        }

        /// <summary>
        /// Remove um palpite pelo seu ID.
        /// </summary>
        public async Task<ApiResponse<bool>> RemoverPalpite(Guid palpiteId)
        {
            try
            {
                var palpiteExistente = await _palpiteRepository.ObterPorId(palpiteId);
                if (palpiteExistente == null)
                {
                    Notificar("Alerta", "Palpite não encontrado para remoção.");
                    return new ApiResponse<bool>(false, "Palpite não encontrado.", false, ObterNotificacoesParaResposta().ToList());
                }

                await _palpiteRepository.Remover(palpiteExistente);

                if (!await CommitAsync())
                {
                    return new ApiResponse<bool>(false, "Falha ao remover palpite.", false, ObterNotificacoesParaResposta().ToList());
                }

                return new ApiResponse<bool>(true, "Palpite removido com sucesso.", true, ObterNotificacoesParaResposta().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover palpite.");
                Notificar("Erro", $"Erro ao remover palpite: {ex.Message}");
                return new ApiResponse<bool>(false, "Ocorreu um erro ao remover o palpite.", false, ObterNotificacoesParaResposta().ToList());
            }
        }

        /// <summary>
        /// Verifica se existem apostas (palpites) para uma rodada específica.
        /// </summary>
        public async Task<ApiResponse<bool>> ExistePalpitesParaRodada(Guid rodadaId)
        {
            try
            {
                var existe = await _palpiteRepository.Buscar(p => p.Jogo.RodadaId == rodadaId).AnyAsync();
                return new ApiResponse<bool>(true, "Verificação de palpites concluída.", existe, ObterNotificacoesParaResposta().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência de palpites para a rodada.");
                Notificar("Erro", $"Erro ao verificar existência de palpites: {ex.Message}");
                return new ApiResponse<bool>(false, "Ocorreu um erro ao verificar palpites.", false, ObterNotificacoesParaResposta().ToList());
            }
        }

        
    }
}
*/