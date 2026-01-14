
// Localização: ApostasApp.Core.Application.Services.Interfaces.Palpites/IPalpiteService.cs

using DevBol.Domain.Models.Apostas;

namespace DevBol.Application.Interfaces.Apostas
{

    public interface IPalpiteService // Não herda diretamente de INotifiableService, mas terá acesso via BaseService
    {
        /// <summary>
        /// Obtém uma coleção de palpites para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Um ApiResponse contendo uma coleção de DTOs de palpites.</returns>
        //Task IEnumerable<PalpiteDto> ObterPalpitesDaRodada(Guid rodadaId);

        Task AdicionarPalpite(Palpite palpite);

        Task AtualizarPalpite(Palpite palpite);

        Task<int> ObterTotaldePontosDoApostadornaRodada(Guid rodadaId, Guid apostadorCampeonatoId);
    
    }

}

        /// <summary>
        /// Atualiza um palpite existente.
        /// </summary>
        /// <param name="palpiteId">O ID do palpite a ser atualizado.</param>
        /// <param name="palpiteRequest">O DTO de requisição contendo os novos dados do palpite.</param>
        /// <returns>Um ApiResponse contendo o DTO do palpite atualizado.</returns>
        //Task<ApiResponse<PalpiteDto>> AtualizarPalpite(Guid palpiteId, SalvarPalpiteRequestDto palpiteRequest);

        /// <summary>
        /// Remove um palpite pelo seu ID.
        /// </summary>
        /// <param name="palpiteId">O ID do palpite a ser removido.</param>
        /// <returns>Um ApiResponse indicando se a remoção foi bem-sucedida.</returns>
        //Task<ApiResponse<bool>> RemoverPalpite(Guid palpiteId);

        /// <summary>
        /// Verifica se existem apostas (palpites) para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Um ApiResponse indicando se existem palpites.</returns>
        //Task<ApiResponse<bool>> ExistePalpitesParaRodada(Guid rodadaId);
  //  }
//}
