using ApostasApp.Core.Domain.Models.Notificacoes;
using DevBol.Application.Base;
using DevBol.Application.Interfaces.Campeonatos;
using DevBol.Domain.Interfaces;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;

namespace DevBol.Application.Services.Campeonatos
{
    public class ApostadorCampeonatoService : BaseService, IApostadorCampeonatoService
    {
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        
        
        public ApostadorCampeonatoService(IApostadorCampeonatoRepository apostadorCampeonatoRepository,
                                 INotificador notificador,
                                 IUnitOfWork uow) : base(notificador, uow)
        {
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(ApostadorCampeonato apostadorCampeonato)
        {
            if (!ExecutarValidacao(new ApostadorCampeonatoValidation(), apostadorCampeonato))
                return;           

              if (_apostadorCampeonatoRepository.Buscar(ec => ec.CampeonatoId == apostadorCampeonato.CampeonatoId 
                && ec.ApostadorId == apostadorCampeonato.ApostadorId).Result.Any())
              {
                  Notificar("Este APOSTADOR já foi associado no CAMPEONATO !!");
                  return;
              }

            await _apostadorCampeonatoRepository.Adicionar(apostadorCampeonato);
        }

        public async Task RemoverEntity(ApostadorCampeonato apostadorCampeonato)
        {          

            await _apostadorCampeonatoRepository.RemoverEntity(apostadorCampeonato);
        
        }

        public async Task Remover(Guid Id)
        {

            await _apostadorCampeonatoRepository.Remover(Id);

        }


      /*  public void Dispose()
        {
            _apostadorCampeonatoRepository?.Dispose();
        }*/
    }
}