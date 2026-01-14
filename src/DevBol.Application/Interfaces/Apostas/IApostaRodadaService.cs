// Localização: ApostasApp.Core.Application/Services/Interfaces/Apostas/IApostaRodadaService.cs

using DevBol.Domain.Models.Apostas;

namespace DevBol.Application.Interfaces.Apostas
{
    public interface IApostaRodadaService //: IDisposable
    {

        /*
        Task<ApostaRodada> GerarApostaRodadaInicial (string apostadorCampeonatoIdString,
                                                     string rodadaIdString,
                                                     bool ehApostaCampeonato,
                                                     string identificadorAposta = null)
        */

        Task Adicionar(ApostaRodada apostaRodada);        
        
        Task Atualizar(ApostaRodada apostaRodada); 
        
        Task Remover(Guid Id);
        
    }
}

/*


using DevBol.Domain.Models.Apostas;
using DevBol.Infrastructure.Data.Migrations;

namespace DevBol.Application.Interfaces.Apostas
{ 
    public interface IApostaRodadaService
    {
        //Task<ApiResponse<ApostaRodadaStatusDto>> ObterStatusApostaRodadaParaUsuario(Guid rodadaId, Guid apostadorCampeonatoId);
        //Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> ObterApostasRodadaPorApostador(Guid rodadaId, Guid apostadorCampeonatoId);
        //Task<ApiResponse<IEnumerable<ApostaJogoVisualizacaoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostadorCampeonatoId);

        // Tipo de retorno corrigido para ApiResponse (sem DTO específico no Data)
        //Task<ApiResponse> SalvarApostas(SalvarApostaRequestDto request);
        //Task<ApiResponse<ApostaRodadaDto>> SalvarApostas(SalvarApostaRequestDto salvarApostaDto);
        // Se você tiver o método GerarApostasRodadaParaTodosApostadores, mantenha-o aqui
        //Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> GerarApostasRodadaParaTodosApostadores(string campeonatoIdString);

        // Se você tiver os métodos Adicionar, Atualizar, MarcarApostaRodadaComoSubmetida, mantenha-os aqui
        //Task<ApiResponse> Adicionar(ApostaRodada apostaRodada);
        async Task Adicionar(ApostaRodada apostarodada);
        async Task Atualizar(ApostaRodada apostarodada);

                
        //Task<ApiResponse> MarcarApostaRodadaComoSubmetida(ApostaRodada apostaRodada); // Assumindo que ApostaRodada é o modelo de domínio
        //Task<ApiResponse<IEnumerable<ApostaJogoVisualizacaoDto>>> ObterApostasDoApostadorNaRodadaParaVisualizacao(Guid rodadaId, Guid apostadorCampeonatoId);
    }
}

*/