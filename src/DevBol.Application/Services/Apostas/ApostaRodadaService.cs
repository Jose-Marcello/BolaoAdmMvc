// Localização: ApostasApp.Core.Application.Services/Apostas/ApostaRodadaService.cs

using AutoMapper;
using DevBol.Application.Base;
using DevBol.Application.Interfaces.Apostas;
using DevBol.Domain.Interfaces;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Jogos;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Infrastructure.Data.Migrations;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using Microsoft.EntityFrameworkCore; // Para .Include()
using Microsoft.Extensions.Logging;

namespace DevBol.Application.Services.Apostas
{
    /// <summary>
    /// ApostaRodadaService é responsável pela lógica de negócio de submissão, consulta e geração de apostas de rodada.
    /// </summary>
    public class ApostaRodadaService : BaseService, IApostaRodadaService
    {
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly IRodadaRepository _rodadaRepository;
        private readonly IJogoRepository _jogoRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ApostaRodadaService> _logger;

        public ApostaRodadaService(
            IApostaRodadaRepository apostaRodadaRepository,
            IPalpiteRepository palpiteRepository,
            IRodadaRepository rodadaRepository,
            IJogoRepository jogoRepository,
            IApostadorCampeonatoRepository apostadorCampeonatoRepository,
            IMapper mapper,
            INotificador notificador,
            IUnitOfWork uow,
            ILogger<ApostaRodadaService> logger)
            : base(notificador, uow)
        {
            _apostaRodadaRepository = apostaRodadaRepository;
            _palpiteRepository = palpiteRepository;
            _rodadaRepository = rodadaRepository;
            _jogoRepository = jogoRepository;
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gera uma ApostaRodada inicial com palpites vazios para todos os jogos de uma rodada específica
        /// para um dado apostador.
        /// </summary>
        /// <param name="apostadorCampeonatoIdString">ID do ApostadorCampeonato.</param>
        /// <param name="rodadaIdString">ID da Rodada.</param>
        /// <param name="ehApostaCampeonato">Indica se esta aposta conta para o campeonato.</param>
        /// <param name="identificadorAposta">Um nome opcional para a aposta (ex: "Minha Aposta Principal").</param>
        /// <returns>ApiResponse com a ApostaRodadaDto gerada ou erros.</returns>
        /// 
        /*
        public async Task<ApostaRodada> GerarApostaRodadaInicial(
            string apostadorCampeonatoIdString,
            string rodadaIdString,
            bool ehApostaCampeonato,
            string identificadorAposta = null)
        {
            var apiResponse = new ApiResponse<ApostaRodadaDto>(false, null);
            try
            {
                if (!Guid.TryParse(apostadorCampeonatoIdString, out Guid apostadorCampeonatoIdGuid) ||
                    !Guid.TryParse(rodadaIdString, out Guid rodadaId))
                {
                    Notificar("Erro", "IDs de apostador ou rodada inválidos.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var rodada = await _rodadaRepository.ObterPorId(rodadaId);
                if (rodada == null)
                {
                    Notificar("Erro", "Rodada não encontrada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                if (!ehApostaCampeonato && rodada.Status != StatusRodada.EmApostas)
                {
                    Notificar("Alerta", "Esta rodada não está aberta para apostas avulsas.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterPorId(apostadorCampeonatoIdGuid);
                if (apostadorCampeonato == null)
                {
                    Notificar("Erro", "Associação Apostador-Campeonato não encontrada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var existingApostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoIdGuid &&
                                                                                     ar.RodadaId == rodadaId &&
                                                                                     ar.EhApostaCampeonato == ehApostaCampeonato)
                                                                     .Include(ar => ar.Palpites)
                                                                         .ThenInclude(p => p.Jogo) // Inclui o Jogo dentro de cada Palpite
                                                                             .ThenInclude(j => j.EquipeCasa) // Inclui EquipeCasa do Jogo
                                                                                 .ThenInclude(ec => ec.Equipe) // Inclui a entidade Equipe da EquipeCasa
                                                                     .Include(ar => ar.Palpites)
                                                                         .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                                             .ThenInclude(j => j.EquipeVisitante) // Inclui EquipeVisitante do Jogo
                                                                                 .ThenInclude(ev => ev.Equipe) // Inclui a entidade Equipe da EquipeVisitante
                                                                     .Include(ar => ar.Palpites)
                                                                         .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                                             .ThenInclude(j => j.Estadio) // Inclui o Estádio do Jogo
                                                                     .FirstOrDefaultAsync();

                if (existingApostaRodada != null)
                {
                    if (!existingApostaRodada.Enviada)
                    {
                        _logger.LogInformation($"ApostaRodada existente (ID: {existingApostaRodada.Id}) encontrada para o apostador {apostadorCampeonatoIdGuid} na rodada {rodadaId}. Retornando para preenchimento.");
                        var existingDto = _mapper.Map<ApostaRodadaDto>(existingApostaRodada);
                        apiResponse.Success = true;
                        apiResponse.Data = existingDto;
                        apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                        return apiResponse;
                    }
                    else
                    {
                        Notificar("Alerta", "Já existe uma ApostaRodada enviada para este apostador nesta rodada.");
                        apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                        return apiResponse;
                    }
                }

                var jogosDaRodada = await _jogoRepository.Buscar(j => j.RodadaId == rodadaId)
                    .Include(j => j.EquipeCasa)
                        .ThenInclude(ec => ec.Equipe)
                    .Include(j => j.EquipeVisitante)
                        .ThenInclude(ec => ec.Equipe)
                    .Include(j => j.Estadio)
                    .ToListAsync();

                if (!jogosDaRodada.Any())
                {
                    Notificar("Alerta", "Nenhum jogo encontrado para esta rodada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var novaApostaRodada = new ApostaRodada
                {
                    ApostadorCampeonatoId = apostadorCampeonatoIdGuid,
                    RodadaId = rodadaId,
                    IdentificadorAposta = identificadorAposta ?? (ehApostaCampeonato ? $"Aposta Campeonato - Rodada {rodada.NumeroRodada}" : $"Aposta Avulsa - Rodada {rodada.NumeroRodada}"),
                    EhApostaCampeonato = ehApostaCampeonato,
                    EhApostaIsolada = !ehApostaCampeonato,
                    CustoPagoApostaRodada = ehApostaCampeonato ? 0 : rodada.CustoApostaRodada,
                    PontuacaoTotalRodada = 0,
                    Enviada = false,
                    DataHoraSubmissao = null,
                    DataCriacao = DateTime.Now
                };

                await _apostaRodadaRepository.Adicionar(novaApostaRodada);
                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var palpites = new List<Palpite>();
                foreach (var jogo in jogosDaRodada)
                {
                    palpites.Add(new Palpite
                    {
                        ApostaRodadaId = novaApostaRodada.Id,
                        JogoId = jogo.Id,
                        PlacarApostaCasa = null,
                        PlacarApostaVisita = null,
                        Pontos = 0
                    });
                }

                await _palpiteRepository.AdicionarRange(palpites);
                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // Re-obter a novaApostaRodada com os palpites e jogos incluídos para o mapeamento completo
                var novaApostaRodadaCompleta = await _apostaRodadaRepository.Buscar(ar => ar.Id == novaApostaRodada.Id)
                                                                             .Include(ar => ar.Palpites)
                                                                                 .ThenInclude(p => p.Jogo)
                                                                                     .ThenInclude(j => j.EquipeCasa)
                                                                                         .ThenInclude(ec => ec.Equipe)
                                                                             .Include(ar => ar.Palpites)
                                                                                 .ThenInclude(p => p.Jogo)
                                                                                     .ThenInclude(j => j.EquipeVisitante)
                                                                                         .ThenInclude(ev => ev.Equipe)
                                                                             .Include(ar => ar.Palpites)
                                                                                 .ThenInclude(p => p.Jogo)
                                                                                     .ThenInclude(j => j.Estadio)
                                                                             .FirstOrDefaultAsync();

                var apostaRodadaDto = _mapper.Map<ApostaRodadaDto>(novaApostaRodadaCompleta);

                apiResponse.Success = true;
                apiResponse.Data = apostaRodadaDto;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar ApostaRodada inicial.");
                Notificar("Erro", $"Erro interno ao gerar ApostaRodada inicial: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        */

        /// <summary>
        /// Gera uma ApostaRodada inicial para todos os apostadores de um campeonato
        /// para a rodada atualmente "Em Apostas". (Função de Admin)
        /// </summary>
        /// 
        /*
        public async Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> GerarApostasRodadaParaTodosApostadores(string campeonatoIdString)
        {
            var apiResponse = new ApiResponse<IEnumerable<ApostaRodadaDto>>(false, null);
            try
            {
                if (!Guid.TryParse(campeonatoIdString, out Guid campeonatoId))
                {
                    Notificar("Erro", "ID do campeonato inválido.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var rodadaEmApostas = await _rodadaRepository.Buscar(r => r.CampeonatoId == campeonatoId && r.Status == StatusRodada.EmApostas)
                                                              .FirstOrDefaultAsync();

                if (rodadaEmApostas == null)
                {
                    Notificar("Alerta", "Nenhuma rodada 'Em Apostas' encontrada para este campeonato.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var apostadoresNoCampeonato = await _apostadorCampeonatoRepository.Buscar(ac => ac.CampeonatoId == campeonatoId)
                                                                                 .ToListAsync();

                if (!apostadoresNoCampeonato.Any())
                {
                    Notificar("Alerta", "Nenhum apostador encontrado neste campeonato.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var generatedApostas = new List<ApostaRodadaDto>();
                foreach (var apostadorCampeonato in apostadoresNoCampeonato)
                {
                    var result = await GerarApostaRodadaInicial(
                        apostadorCampeonatoIdString: apostadorCampeonato.Id.ToString(),
                        rodadaIdString: rodadaEmApostas.Id.ToString(),
                        ehApostaCampeonato: true,
                        identificadorAposta: $"Aposta Campeonato - Rodada {rodadaEmApostas.NumeroRodada}"
                    );

                    if (result.Success && result.Data != null)
                    {
                        generatedApostas.Add(result.Data);
                    }
                    else
                    {
                        _logger.LogWarning($"Falha ao gerar ApostaRodada para apostador {apostadorCampeonato.Id}: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");
                        Notificar("Alerta", $"Falha ao gerar aposta para um apostador: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");
                    }
                }

                apiResponse.Success = true;
                apiResponse.Data = generatedApostas;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar ApostasRodada para todos os apostadores.");
                Notificar("Erro", $"Erro interno ao gerar ApostasRodada para todos os apostadores: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }
       
       */


        public async Task Adicionar(ApostaRodada apostaRodada)
        {
            if (!ExecutarValidacao(new ApostaRodadaValidation(), apostaRodada))
                return;
                        
            await _apostaRodadaRepository.Adicionar(apostaRodada);
        }


        public async Task Remover(Guid id)
        {            
            await _apostaRodadaRepository.Remover(id);
        }


        public async Task Atualizar(ApostaRodada apostaRodada)
        {
            if (!ExecutarValidacao(new ApostaRodadaValidation(), apostaRodada)) return;

            /* if (_apostaRepository.Buscar(r => r.NumeroAposta == aposta.NumeroAposta
               && r.CampeonatoId == aposta.CampeonatoId).Result.Any())
             { 

                 Notificar("Já existe uma aposta neste campeonato com este NÚMERO informado.");
                 return;
              }
            */

          await _apostaRodadaRepository.Atualizar(apostaRodada);

      }



      /*

      /// <summary>
      /// Atualiza uma ApostaRodada existente.
      /// </summary>
      public async Task<ApiResponse> Atualizar(ApostaRodada apostaRodada)
      {
          var apiResponse = new ApiResponse(false, null);
          try
          {
              await _apostaRodadaRepository.Atualizar(apostaRodada);
              if (!await CommitAsync())
              {
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }
              apiResponse.Success = true;
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Erro ao atualizar ApostaRodada.");
              Notificar("Erro", $"Erro interno ao atualizar ApostaRodada: {ex.Message}");
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
      }

      /// <summary>
      /// Marca uma ApostaRodada como submetida, registrando a data e hora.
      /// </summary>
      public async Task<ApiResponse> MarcarApostaRodadaComoSubmetida(ApostaRodada apostaRodada)
      {
          var apiResponse = new ApiResponse(false, null);
          try
          {
              apostaRodada.DataHoraSubmissao = DateTime.UtcNow;
              apostaRodada.Enviada = true;
              await _apostaRodadaRepository.Atualizar(apostaRodada);
              if (!await CommitAsync())
              {
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }
              apiResponse.Success = true;
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Erro ao marcar ApostaRodada como submetida.");
              Notificar("Erro", $"Erro interno ao marcar ApostaRodada como submetida: {ex.Message}");
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
      }

      /// <summary>
      /// Obtém o status de envio e a data/hora da aposta de uma rodada para um usuário específico.
      /// </summary>
      public async Task<ApiResponse<ApostaRodadaStatusDto>> ObterStatusApostaRodadaParaUsuario(Guid rodadaId, Guid apostadorCampeonatoId)
      {
          try
          {
              var apostaRodada = await _apostaRodadaRepository.ObterStatusApostaRodada(rodadaId, apostadorCampeonatoId);

              ApostaRodadaStatusDto statusDto;

              if (apostaRodada != null)
              {
                  statusDto = _mapper.Map<ApostaRodadaStatusDto>(apostaRodada);
                  statusDto.StatusAposta = 1;
              }
              else
              {
                  statusDto = new ApostaRodadaStatusDto
                  {
                      RodadaId = rodadaId.ToString(),
                      ApostadorCampeonatoId = apostadorCampeonatoId.ToString(),
                      StatusAposta = 0,
                      DataAposta = null
                  };
              }

              return new ApiResponse<ApostaRodadaStatusDto>
              {
                  Success = true,
                  Message = "Status da aposta da rodada obtido com sucesso.",
                  Data = statusDto,
                  Notifications = new List<NotificationDto>()
              };
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Erro ao obter status da aposta da rodada.");

              Notificar("Erro", $"Erro ao obter status da aposta da rodada: {ex.Message}");

              return new ApiResponse<ApostaRodadaStatusDto>
              {
                  Success = false,
                  Message = "Ocorreu um erro interno ao obter o status da aposta da rodada.",
                  Data = null,
                  Notifications = ObterNotificacoesParaResposta().ToList()
              };
          }
      }

      /// <summary>
      /// Obtém as apostas de um apostador em uma rodada, formatadas para edição.
      /// Este método agora retorna uma lista de ApostaJogoVisualizacaoDto, com dados completos do jogo e palpites.
      /// </summary>
      public async Task<ApiResponse<IEnumerable<ApostaJogoVisualizacaoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostadorCampeonatoId)
      {
          var apiResponse = new ApiResponse<IEnumerable<ApostaJogoVisualizacaoDto>>(false, null);
          try
          {
              _logger.LogInformation($"[ParaEdicao] Iniciando ObterApostasDoApostadorNaRodadaParaEdicao para RodadaId: {rodadaId}, ApostadorCampeonatoId: {apostadorCampeonatoId}");

              // 1. Obter todos os jogos da rodada com suas equipes e escudos
              var jogosDaRodada = await _jogoRepository.Buscar(j => j.RodadaId == rodadaId)
                  .Include(j => j.EquipeCasa)
                      .ThenInclude(ec => ec.Equipe)
                  .Include(j => j.EquipeVisitante)
                      .ThenInclude(ev => ev.Equipe)
                  .Include(j => j.Estadio)
                  .ToListAsync();

              _logger.LogInformation($"[ParaEdicao] Encontrados {jogosDaRodada.Count} jogos para a rodada {rodadaId}.");

              if (!jogosDaRodada.Any())
              {
                  Notificar("Alerta", "Nenhum jogo encontrado para esta rodada.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }

              // 2. Tentar obter a aposta da rodada existente para o apostador, incluindo os palpites
              // <<-- FOCO DA DEPURACAO AQUI -->>
              var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.RodadaId == rodadaId && ar.ApostadorCampeonatoId == apostadorCampeonatoId)
                                                               .Include(ar => ar.Palpites) // Incluir os palpites
                                                               .FirstOrDefaultAsync();

              if (apostaRodada != null)
              {
                  _logger.LogInformation($"[ParaEdicao] ApostaRodada encontrada (ID: {apostaRodada.Id}) para o apostador {apostadorCampeonatoId}. Total de palpites: {apostaRodada.Palpites.Count()}.");
                  foreach (var palpite in apostaRodada.Palpites)
                  {
                      _logger.LogInformation($"[ParaEdicao] Palpite ID: {palpite.Id}, Jogo ID: {palpite.JogoId}, Placar Casa: {palpite.PlacarApostaCasa}, Placar Visita: {palpite.PlacarApostaVisita}");
                  }
              }
              else
              {
                  _logger.LogInformation($"[ParaEdicao] Nenhuma ApostaRodada encontrada para o apostador {apostadorCampeonatoId} na rodada {rodadaId}.");
              }


              // 3. Obter a rodada para acessar o CampeonatoId (se necessário, embora não esteja no DTO final)
              var rodada = await _rodadaRepository.ObterPorId(rodadaId);
              if (rodada == null)
              {
                  Notificar("Erro", "Rodada não encontrada para obter CampeonatoId.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }

              var jogosVisualizacao = new List<ApostaJogoVisualizacaoDto>();

              // 4. Iterar sobre os jogos da rodada e criar os DTOs de visualização
              foreach (var jogo in jogosDaRodada.OrderBy(j => j.DataJogo).ThenBy(j => j.Id)) // Ordenar por Id do jogo para consistência
              {
                  var palpiteExistente = apostaRodada?.Palpites.FirstOrDefault(p => p.JogoId == jogo.Id);

                  _logger.LogInformation($"[ParaEdicao] Processando Jogo ID: {jogo.Id}. Palpite Existente: {(palpiteExistente != null ? "Sim" : "Não")}.");
                  if (palpiteExistente != null)
                  {
                      _logger.LogInformation($"[ParaEdicao] Palpite Existente para Jogo {jogo.Id}: Placar Casa: {palpiteExistente.PlacarApostaCasa}, Placar Visita: {palpiteExistente.PlacarApostaVisita}.");
                  }
                  else
                  {
                      _logger.LogInformation($"[ParaEdicao] Nenhum palpite existente encontrado para Jogo ID: {jogo.Id}.");
                  }

                  jogosVisualizacao.Add(new ApostaJogoVisualizacaoDto
                  {
                      Id = palpiteExistente?.Id.ToString() ?? Guid.Empty.ToString(), // ID do palpite, se existir, ou Guid.Empty
                      IdJogo = jogo.Id.ToString(), // ID do jogo

                      EquipeMandante = jogo.EquipeCasa.Equipe.Nome,
                      SiglaMandante = jogo.EquipeCasa.Equipe.Sigla,
                      EscudoMandante = jogo.EquipeCasa.Equipe.Escudo, // Assumindo que Escudo é o URL

                      PlacarRealCasa = jogo.PlacarCasa, // Placar oficial do jogo
                      PlacarApostaCasa = palpiteExistente?.PlacarApostaCasa, // Palpite do usuário

                      EquipeVisitante = jogo.EquipeVisitante.Equipe.Nome,
                      SiglaVisitante = jogo.EquipeVisitante.Equipe.Sigla,
                      EscudoVisitante = jogo.EquipeVisitante.Equipe.Escudo,

                      PlacarRealVisita = jogo.PlacarVisita, // Placar oficial do jogo
                      PlacarApostaVisita = palpiteExistente?.PlacarApostaVisita, // Palpite do usuário

                      DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"), // Formato "AAAA-MM-DD"
                      HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm"), // Formato "HH:MM"

                      StatusJogo = jogo.Status.ToString(), // Assumindo que Jogo tem um Status
                      Enviada = apostaRodada?.Enviada ?? false, // Se a ApostaRodada foi enviada
                      Pontuacao = palpiteExistente?.Pontos ?? 0,// Pontos ganhos no palpite

                      EstadioNome = jogo.Estadio.Nome

                  });
              }

              apiResponse.Success = true;
              apiResponse.Data = jogosVisualizacao;
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              _logger.LogInformation($"[ParaEdicao] ObterApostasDoApostadorNaRodadaParaEdicao finalizado. Retornando {jogosVisualizacao.Count} jogos visualização.");
              return apiResponse;
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Erro ao obter apostas do apostador na rodada para edição.");
              Notificar("Erro", $"Erro interno ao obter apostas para edição: {ex.Message}");
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
      }

      /// <summary>
      /// Obtém as apostas de um apostador em uma rodada, formatadas para visualização (com placares reais e pontuação).
      /// </summary>
      public async Task<ApiResponse<IEnumerable<ApostaJogoVisualizacaoDto>>> ObterApostasDoApostadorNaRodadaParaVisualizacao(Guid rodadaId, Guid apostadorCampeonatoId)
      {
          var apiResponse = new ApiResponse<IEnumerable<ApostaJogoVisualizacaoDto>>(false, null);
          try
          {
              var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.RodadaId == rodadaId && ar.ApostadorCampeonatoId == apostadorCampeonatoId)
                                                               .Include(ar => ar.Palpites)
                                                               .FirstOrDefaultAsync();

              // Assumindo que ObterJogosDaRodadaComPlacaresEEquipes carrega os jogos com equipes e placares oficiais
              var jogosDaRodada = await _jogoRepository.Buscar(j => j.RodadaId == rodadaId)
                  .Include(j => j.EquipeCasa)
                      .ThenInclude(ec => ec.Equipe)
                  .Include(j => j.EquipeVisitante)
                      .ThenInclude(ev => ev.Equipe)
                  .Include(j => j.Estadio)
                  .ToListAsync();

              if (!jogosDaRodada.Any())
              {
                  Notificar("Alerta", "Não há jogos definidos para esta rodada.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }

              var apostasParaVisualizacao = new List<ApostaJogoVisualizacaoDto>();

              foreach (var jogo in jogosDaRodada.OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo))
              {
                  var palpite = apostaRodada?.Palpites.FirstOrDefault(p => p.JogoId == jogo.Id);

                  apostasParaVisualizacao.Add(new ApostaJogoVisualizacaoDto
                  {
                      Id = palpite?.Id.ToString(),
                      IdJogo = jogo.Id.ToString(),
                      // Removido CampeonatoId e RodadaId para corresponder ao seu DTO original
                      DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"),
                      HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm"),

                      EquipeMandante = jogo.EquipeCasa.Equipe.Nome,
                      SiglaMandante = jogo.EquipeCasa.Equipe.Sigla,
                      EscudoMandante = jogo.EquipeCasa.Equipe.Escudo,

                      PlacarRealCasa = jogo.PlacarCasa,
                      PlacarApostaCasa = palpite?.PlacarApostaCasa,

                      EquipeVisitante = jogo.EquipeVisitante.Equipe.Nome,
                      SiglaVisitante = jogo.EquipeVisitante.Equipe.Sigla,
                      EscudoVisitante = jogo.EquipeVisitante.Equipe.Escudo,
                      EstadioNome = jogo.Estadio.Nome,

                      PlacarRealVisita = jogo.PlacarVisita,
                      PlacarApostaVisita = palpite?.PlacarApostaVisita,

                      StatusJogo = jogo.Status.ToString(), // Pode ser o status do jogo ou status da aposta no jogo
                      Enviada = apostaRodada?.Enviada ?? false,
                      Pontuacao = palpite?.Pontos ?? 0
                  });
              }

              apiResponse.Success = true;
              apiResponse.Data = apostasParaVisualizacao;
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Erro ao obter apostas do apostador na rodada para visualização.");
              Notificar("Erro", $"Erro interno ao obter apostas para visualização: {ex.Message}");
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
      }

      /// <summary>
      /// Obtém as apostas de um apostador em uma rodada.
      /// Este método é para listar as "ApostasRodada" de um apostador para uma rodada específica.
      /// </summary>
      public async Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> ObterApostasRodadaPorApostador(Guid rodadaId, Guid apostadorCampeonatoId)
      {
          var apiResponse = new ApiResponse<IEnumerable<ApostaRodadaDto>>(false, null);
          try
          {
              var apostasRodada = await _apostaRodadaRepository.Buscar(ar =>
                                              ar.RodadaId == rodadaId &&
                                              ar.ApostadorCampeonatoId == apostadorCampeonatoId)
                                              .Include(ar => ar.Palpites)
                                                  .ThenInclude(p => p.Jogo) // Inclui o Jogo dentro de cada Palpite
                                                      .ThenInclude(j => j.EquipeCasa) // Inclui EquipeCasa do Jogo
                                                          .ThenInclude(ec => ec.Equipe) // Inclui a entidade Equipe da EquipeCasa
                                              .Include(ar => ar.Palpites)
                                                  .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                      .ThenInclude(j => j.EquipeVisitante) // Inclui EquipeVisitante do Jogo
                                                          .ThenInclude(ev => ev.Equipe) // Inclui a entidade Equipe da EquipeVisitante
                                              .Include(ar => ar.Palpites)
                                                  .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                      .ThenInclude(j => j.Estadio) // Inclui o Estádio do Jogo
                                              .ToListAsync();

              if (!apostasRodada.Any())
              {
                  Notificar("Alerta", "Nenhuma aposta de rodada encontrada para este apostador nesta rodada.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }

              var apostasRodadaDto = _mapper.Map<IEnumerable<ApostaRodadaDto>>(apostasRodada);

              apiResponse.Success = true;
              apiResponse.Data = apostasRodadaDto;
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Erro ao obter apostas de rodada por apostador.");
              Notificar("Erro", $"Erro interno ao obter apostas de rodada por apostador: {ex.Message}");
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
      }

      /// <summary>
      /// Salva ou atualiza as apostas de uma rodada para um apostador.
      /// </summary>
      public async Task<ApiResponse<ApostaRodadaDto>> SalvarApostas(SalvarApostaRequestDto salvarApostaDto)
      {
          // O tipo de retorno da ApiResponse foi alterado para ApostaRodadaDto
          var apiResponse = new ApiResponse<ApostaRodadaDto>(false, null);
          try
          {
              _logger.LogInformation("Iniciando SalvarApostas.");

              if (salvarApostaDto == null || !salvarApostaDto.ApostasJogos.Any())
              {
                  Notificar("Alerta", "Nenhuma aposta foi enviada para salvar.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }

              var rodada = await _rodadaRepository.ObterPorId(Guid.Parse(salvarApostaDto.RodadaId));
              if (rodada == null)
              {
                  Notificar("Erro", "Rodada não encontrada.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }
              if (rodada.Status != StatusRodada.EmApostas)
              {
                  Notificar("Alerta", "Não é possível salvar apostas para esta rodada. Ela não está mais 'Em Apostas'.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }

              // --- VALIDAÇÃO DA REGRA DE NEGÓCIO: Ao menos 3 empates/vitória dos visitantes ---
              int empates = 0;
              int vitoriasVisitante = 0;

              foreach (var apostaJogoDto in salvarApostaDto.ApostasJogos)
              {
                  // Certifique-se de que os placares não são nulos para a validação
                  if (apostaJogoDto.PlacarCasa.HasValue && apostaJogoDto.PlacarVisitante.HasValue)
                  {
                      if (apostaJogoDto.PlacarCasa.Value == apostaJogoDto.PlacarVisitante.Value)
                      {
                          empates++;
                      }
                      else if (apostaJogoDto.PlacarVisitante.Value > apostaJogoDto.PlacarCasa.Value)
                      {
                          vitoriasVisitante++;
                      }
                  }
              }

              if ((empates + vitoriasVisitante) < 3)
              {
                  Notificar("Alerta", "Sua aposta deve conter no mínimo 3 resultados de empate ou vitória do time visitante.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse; // Retorna com erro de validação
              }
              // --- FIM DA VALIDAÇÃO ---

              // Busca a apostaRodada existente ou cria uma nova
              // Inclui os palpites existentes para poder atualizá-los
              var apostaRodada = await _apostaRodadaRepository.Buscar(ar =>
                                          ar.RodadaId == Guid.Parse(salvarApostaDto.RodadaId) &&
                                          ar.ApostadorCampeonatoId == Guid.Parse(salvarApostaDto.ApostadorCampeonatoId))
                                          .Include(ar => ar.Palpites) // Inclui palpites para poder iterar e atualizar
                                          .FirstOrDefaultAsync();

              if (apostaRodada == null)
              {
                  apostaRodada = new ApostaRodada
                  {
                      RodadaId = Guid.Parse(salvarApostaDto.RodadaId),
                      ApostadorCampeonatoId = Guid.Parse(salvarApostaDto.ApostadorCampeonatoId),
                      EhApostaCampeonato = salvarApostaDto.EhCampeonato,
                      EhApostaIsolada = false, // Assumindo que apostas salvas aqui não são isoladas por padrão
                      Enviada = false, // Será marcado como true mais abaixo
                      PontuacaoTotalRodada = 0,
                      DataCriacao = DateTime.Now,
                      IdentificadorAposta = salvarApostaDto.IdentificadorAposta
                  };
                  await _apostaRodadaRepository.Adicionar(apostaRodada);
              }

              apostaRodada.DataHoraSubmissao = DateTime.UtcNow; // Define a data de submissão
              apostaRodada.Enviada = true; // Marca como enviada ao salvar

              // Atualiza a apostaRodada principal (para salvar DataHoraSubmissao e Enviada)
              await _apostaRodadaRepository.Atualizar(apostaRodada);

              // Itera sobre os palpites enviados e atualiza/cria
              foreach (var apostaJogoDto in salvarApostaDto.ApostasJogos)
              {
                  var jogoId = Guid.Parse(apostaJogoDto.JogoId);
                  var palpite = apostaRodada.Palpites.FirstOrDefault(p => p.JogoId == jogoId);

                  if (palpite == null)
                  {
                      palpite = new Palpite
                      {
                          ApostaRodadaId = apostaRodada.Id,
                          JogoId = jogoId,
                          PlacarApostaCasa = apostaJogoDto.PlacarCasa,
                          PlacarApostaVisita = apostaJogoDto.PlacarVisitante,
                          Pontos = 0 // Pontos iniciais
                      };
                      await _palpiteRepository.Adicionar(palpite);
                  }
                  else
                  {
                      palpite.PlacarApostaCasa = apostaJogoDto.PlacarCasa;
                      palpite.PlacarApostaVisita = apostaJogoDto.PlacarVisitante;
                      await _palpiteRepository.Atualizar(palpite);
                  }
              }

              // Salva todas as alterações no banco de dados
              if (!await CommitAsync())
              {
                  Notificar("Erro", "Falha ao persistir dados da aposta.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }

              // --- RETORNA A APOSTA ATUALIZADA COM PALPITES E JOGOS ---
              // Rebusca a apostaRodada com todos os includes necessários para o DTO de retorno
              var apostaRodadaCompleta = await _apostaRodadaRepository.Buscar(ar => ar.Id == apostaRodada.Id)
                                                                      .Include(ar => ar.Palpites)
                                                                          .ThenInclude(p => p.Jogo) // Inclui o Jogo dentro de cada Palpite
                                                                              .ThenInclude(j => j.EquipeCasa) // Inclui EquipeCasa do Jogo
                                                                                  .ThenInclude(ec => ec.Equipe) // Inclui a entidade Equipe da EquipeCasa
                                                                      .Include(ar => ar.Palpites)
                                                                          .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                                              .ThenInclude(j => j.EquipeVisitante) // Inclui EquipeVisitante do Jogo
                                                                                  .ThenInclude(ev => ev.Equipe) // Inclui a entidade Equipe da EquipeVisitante
                                                                      .Include(ar => ar.Palpites)
                                                                          .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                                              .ThenInclude(j => j.Estadio) // Inclui o Estádio do Jogo
                                                                      .FirstOrDefaultAsync();

              if (apostaRodadaCompleta == null)
              {
                  Notificar("Erro", "Aposta salva, mas não foi possível recuperá-la para retorno.");
                  apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                  return apiResponse;
              }

              // Mapeia a entidade completa para o DTO de retorno
              apiResponse.Data = _mapper.Map<ApostaRodadaDto>(apostaRodadaCompleta);
              apiResponse.Success = true;
              apiResponse.Message = "Apostas salvas com sucesso!";
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList(); // Inclui quaisquer notificações que possam ter sido adicionadas
              return apiResponse;
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Erro ao salvar apostas.");
              Notificar("Erro", $"Erro interno ao salvar apostas: {ex.Message}");
              apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
              return apiResponse;
          }
      }
      */
        }
    }
