using DevBol.Application.Interfaces.Equipes;
using DevBol.Application.Base;
using DevBol.Domain.Models.Equipes;
using DevBol.Domain.Interfaces;
using DevBol.Infrastructure.Data.Interfaces.Equipes;

namespace DevBol.Application.Services.Equipes
{
    public class EquipeService : BaseService, IEquipeService
    {
        private readonly IEquipeRepository _equipeRepository;
        
        
        public EquipeService(IEquipeRepository equipeRepository,
                                 INotificador notificador,
                                 IUnitOfWork uow) : base(notificador, uow)
        {
            _equipeRepository = equipeRepository;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(Equipe equipe)
        {
            if (!ExecutarValidacao(new EquipeValidation(), equipe))
                return;           

              if (_equipeRepository.Buscar(e => e.Nome == equipe.Nome).Result.Any())
              {
                  Notificar("Já existe uma EQUIPE com este NOME infomado.");
                  return;
              }

            await _equipeRepository.Adicionar(equipe);
        }

        public async Task Atualizar(Equipe equipe)
        {
            if (!ExecutarValidacao(new EquipeValidation(), equipe)) return;
                        

            await _equipeRepository.Atualizar(equipe);
        }

        public async Task Remover(Guid id)
        {
            //var campeonato = await _campeonatoRepository.ObterRodadasDoCampeonato(id);

            //if (campeonato.Rodadas.Any())
            //{
            //    Notificar("O campeonato possui RODADAS associadas !");
            //    return;
            //}

            await _equipeRepository.Remover(id);
        }


        public async Task RemoverEntity(Equipe equipe)
        {
            //var campeonato = await _campeonatoRepository.ObterRodadasDoCampeonato(id);

            //if (campeonato.Rodadas.Any())
            //{
            //    Notificar("O campeonato possui RODADAS associadas !");
            //    return;
            //}

            await _equipeRepository.RemoverEntity(equipe);
        }

      /*  public void Dispose()
        {
            _equipeRepository?.Dispose();
        }*/
    }
}