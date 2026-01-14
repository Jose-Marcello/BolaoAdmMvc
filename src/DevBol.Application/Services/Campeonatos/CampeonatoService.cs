using DevBol.Application.Interfaces.Campeonatos;
using DevBol.Application.Base;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Interfaces;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;

namespace DevBol.Application.Services.Campeonatos
{
    public class CampeonatoService : BaseService, ICampeonatoService
    {
        private readonly ICampeonatoRepository _campeonatoRepository;
        

        public CampeonatoService(ICampeonatoRepository campeonatoRepository,
                                 INotificador notificador,
                                 IUnitOfWork uow) : base(notificador,uow )
        {
            _campeonatoRepository = campeonatoRepository;
        }

        public async Task Adicionar(Campeonato campeonato) 
        {
            if (!ExecutarValidacao(new CampeonatoValidation(), campeonato))
                return;

            if (_campeonatoRepository.Buscar(f => f.Nome == campeonato.Nome).Result.Any())
            {
                Notificar("Já existe um campeonato com este NOME infomado.");
                return;
            }

            await _campeonatoRepository.Adicionar(campeonato);
        }

        public async Task Atualizar(Campeonato campeonato)
        {
            if (!ExecutarValidacao(new CampeonatoValidation(), campeonato)) return;

            if (_campeonatoRepository.Buscar(f => f.Nome == campeonato.Nome && f.Id != campeonato.Id).Result.Any())
            {
                Notificar("Já existe um campeonato com este NOME informado.");
                return;
            }

            await _campeonatoRepository.Atualizar(campeonato);
        }

        public async Task Remover(Guid id)
        {
            //var campeonato = await _campeonatoRepository.ObterRodadasDoCampeonato(id);

            //if (campeonato.Rodadas.Any())
            //{
            //    Notificar("O campeonato possui RODADAS associadas !");
            //    return;
            //}

            await _campeonatoRepository.Remover(id);
        }

        public async Task RemoverEntity(Campeonato campeonato)
        {
            //var campeonato = await _campeonatoRepository.ObterRodadasDoCampeonato(id);

            //if (campeonato.Rodadas.Any())
            //{
            //    Notificar("O campeonato possui RODADAS associadas !");
            //    return;
            //}

            await _campeonatoRepository.RemoverEntity(campeonato);
        }

       /* public void Dispose()
        {
            _campeonatoRepository?.Dispose();
        }*/
    }
}