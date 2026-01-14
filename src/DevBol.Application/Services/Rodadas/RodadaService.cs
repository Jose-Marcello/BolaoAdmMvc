using DevBol.Application.Interfaces.Rodadas;
using DevBol.Application.Base;
using DevBol.Domain.Models.Rodadas;
using DevBol.Domain.Interfaces;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Interfaces.Jogos;
using DevBol.Domain.Models.Jogos;


namespace DevBol.Application.Services.Rodadas
{
    public class RodadaService : BaseService, IRodadaService
    {
        private readonly IRodadaRepository _rodadaRepository;
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly IJogoRepository _jogoRepository;


        public RodadaService(IRodadaRepository rodadaRepository,
                             IApostaRodadaRepository apostaRodadaRepository,
                             IJogoRepository   jogoRepository,
                             IUnitOfWork uow,
                             INotificador notificador) : base(notificador, uow)
        {
            _rodadaRepository = rodadaRepository;
            _apostaRodadaRepository = apostaRodadaRepository;
            _jogoRepository = jogoRepository;

        }


        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(Rodada rodada)
        {
            if (!ExecutarValidacao(new RodadaValidation(), rodada))
                return;
                      

            if (_rodadaRepository.Buscar(r => r.NumeroRodada == rodada.NumeroRodada 
                && r.CampeonatoId == rodada.CampeonatoId).Result.Any())
              {
                  Notificar("Já existe uma RODADA neste CAMPEONATO com este NÚMERO infomado.");
                  return;
              }

            

            await _rodadaRepository.Adicionar(rodada);
        }

        public async Task Atualizar(Rodada rodada)
        {
            if (!ExecutarValidacao(new RodadaValidation(), rodada)) return;

            /* if (_rodadaRepository.Buscar(r => r.NumeroRodada == rodada.NumeroRodada
               && r.CampeonatoId == rodada.CampeonatoId).Result.Any())
             { 

                 Notificar("Já existe uma rodada neste campeonato com este NÚMERO informado.");
                 return;
              }*/

            if (rodada.Status == StatusRodada.NaoIniciada)
            {
                var numeroJogosEsperado = rodada.NumJogos; // Supondo que sua entidade Rodada tenha essa propriedade
                var jogosAssociados = await _jogoRepository.ObterJogosDaRodada(rodada.Id);
                var numeroJogosAssociados = jogosAssociados.Count();

                if (numeroJogosAssociados != numeroJogosEsperado)
                {
                    Notificar("Essa rodada não pode ser colocada PRONTA e NÃO INCIADA !! Ainda não existem JOGOS a serem associados à RODADA !!");
                    return;
                    //return Result.Failure($"A rodada não pode ser definida como 'Pronta para Iniciar'. Número de jogos associados ({numeroJogosAssociados}) não corresponde ao número de jogos esperado ({numeroJogosEsperado}).");
                }
            }

            if (rodada.Status == StatusRodada.Finalizada)
            {    
                var temJogoNaoFinalizado = await _jogoRepository.ExistemJogosNaoFinalizadosNaRodada(rodada.Id);

                if (temJogoNaoFinalizado)
                {
                    Notificar("Essa rodada não pode ser FINALIZADA !! Ainda existem JOGOS a serem FINALIZADOS !!");
                    return;

                }
              
                /*
                var jogosAssociados = await _jogoRepository.ObterJogosDaRodada(rodada.Id);
                //var numeroJogosAssociados = jogosAssociados.Count();

                var jogosNaoFinalizados = 0;

                foreach(var jogo in jogosAssociados)
                {
                    if (jogo.Status !=  StatusJogo.Finalizado){

                        jogosNaoFinalizados = jogosNaoFinalizados + 1;                            
                            
                    }
                }                    
                
                if (jogosNaoFinalizados > 0)
                {
                    Notificar("Essa rodada não pode ser FINALIZADA !! Ainda existem " + jogosNaoFinalizados + " JOGOS a serem FINALIZADOS !!");
                    return;
                    //return Result.Failure($"A rodada não pode ser definida como 'Pronta para Iniciar'. Número de jogos associados ({numeroJogosAssociados}) não corresponde ao número de jogos esperado ({numeroJogosEsperado}).");
                }
                */
            }

            var TemApostasGeradas = await _apostaRodadaRepository.ExisteApostaNaRodada(rodada.Id); 

            if (rodada.Status == StatusRodada.EmApostas && !TemApostasGeradas)
            {
                Notificar("Essa rodada não pode ser colocada EM APOSTAS !! Ainda não existem APOSTAS GERADAS para esta RODADA !!");
                return;
            }

            await _rodadaRepository.Atualizar(rodada);
        }

        public async Task Remover(Guid id)
        {
            //aqui tem que verificar se esta RODADA já tem JOGOS associados
            /*var rodada = await _rodadaRepository.obterJogosDaRodada(id);

            if (rodada.Any())
            {
                Notificar("O RODADA possui JOGOS associados ! Não pode ser excluída ");
                return;
            }*/

            await _rodadaRepository.Remover(id);
        }

        public void Dispose()
        {
            _rodadaRepository?.Dispose();
        }
    }
}