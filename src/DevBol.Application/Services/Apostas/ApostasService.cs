/*
using DevBol.Application.Base;
using DevBol.Application.Interfaces.Apostas;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Interfaces;
using DevBol.Infrastructure.Data.Interfaces.Apostas;

namespace DevBol.Application.Services.Apostas
{
    public class ApostaService : BaseService, IApostaService
    {
        private readonly IApostaRepository _apostaRepository;        
        
        public ApostaService(IApostaRepository apostaRepository,
                                 INotificador notificador) : base(notificador)
        {
            _apostaRepository = apostaRepository;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(Aposta aposta)
        {
            if (!ExecutarValidacao(new ApostaValidation(), aposta))
                return;           

              if (_apostaRepository.Buscar(a => a.JogoId == aposta.JogoId 
                && a.ApostadorCampeonatoId == aposta.ApostadorCampeonatoId).Result.Any())
              {
                  Notificar("Já existe uma APOSTA deste APOSTADOR para este JOGO !!");
                  return;
              }

            await _apostaRepository.Adicionar(aposta);
        }

        public async Task Atualizar(Aposta aposta)
        {
            if (!ExecutarValidacao(new ApostaValidation(), aposta)) return;

              /* if (_apostaRepository.Buscar(r => r.NumeroAposta == aposta.NumeroAposta
                 && r.CampeonatoId == aposta.CampeonatoId).Result.Any())
               { 
                
                   Notificar("Já existe uma aposta neste campeonato com este NÚMERO informado.");
                   return;
                }

            await _apostaRepository.Atualizar(aposta);
        }

        public async Task Remover(Guid id)
        {
            //aqui tem que verificar se esta RODADA já tem JOGOS associados
            /*var aposta = await _apostaRepository.obterJogosDaAposta(id);

            if (aposta.Any())
            {
                Notificar("O RODADA possui JOGOS associados ! Não pode ser excluída ");
                return;
            }

            await _apostaRepository.Remover(id);
        }

       /* public void Dispose()
        {
            _apostaRepository?.Dispose();
        }
    }
}
*/