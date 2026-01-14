using DevBol.Application.Interfaces.Estadios;
using DevBol.Application.Base;
using DevBol.Domain.Models.Estadios;
using DevBol.Domain.Models.Estados;
using DevBol.Domain.Interfaces;
using DevBol.Infrastructure.Data.Interfaces.Estadios;

namespace DevBol.Application.Services.Equipes
{
    public class EstadioService : BaseService, IEstadioService
    {
        private readonly IEstadioRepository _estadioRepository;
        
        
        public EstadioService(IEstadioRepository estadioRepository,
                                 INotificador notificador,
                                 IUnitOfWork uow) : base(notificador, uow)
        {
            _estadioRepository = estadioRepository;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(Estadio estadio)
        {
            if (!ExecutarValidacao(new EstadioValidation(), estadio))
                return;           

              if (_estadioRepository.Buscar(e => e.Nome == estadio.Nome).Result.Any())
              {
                  Notificar("Já existe uma Estádio com este NOME informado.");
                  return;
              }

            await _estadioRepository.Adicionar(estadio);
        }

        public async Task Atualizar(Estadio estadio)
        {
            if (!ExecutarValidacao(new EstadioValidation(), estadio)) return;
                        

            await _estadioRepository.Atualizar(estadio);
        }

        public async Task Remover(Guid id)
        {
            //var campeonato = await _campeonatoRepository.ObterRodadasDoCampeonato(id);

            //if (campeonato.Rodadas.Any())
            //{
            //    Notificar("O campeonato possui RODADAS associadas !");
            //    return;
            //}

            await _estadioRepository.Remover(id);
        }

        public async Task RemoverEntity(Estadio estadio)
        {
            //var campeonato = await _campeonatoRepository.ObterRodadasDoCampeonato(id);

            //if (campeonato.Rodadas.Any())
            //{
            //    Notificar("O campeonato possui RODADAS associadas !");
            //    return;
            //}

            await _estadioRepository.RemoverEntity(estadio);
        }


        /*public void Dispose()
        {
            _estadioRepository?.Dispose();
        }*/
    }
}