using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DevBol.Presentation.ViewModels;
using DevBol.Domain.Models.Rodadas;
using DevBol.Application.Interfaces.Jogos;
using DevBol.Application.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.RankingRodadas;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Domain.Interfaces;
using DevBol.Application;
using DevBol.Domain.Models.RankingRodadas;
using DevBol.Infrastructure.Data.Repository.RankingRodadas;
using DevBol.Infrastructure.Data.Repository.Rodadas;
using DevBol.Domain.Models.Campeonatos;



namespace DevBol.Presentation.Controllers
{
    [Route("RankingRodada")]
    public class RankingRodadaController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;               
       
        public RankingRodadaController(IMapper mapper,
                                       IUnitOfWork uow, 
                                       INotificador notificador) : base(notificador)
        //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;
            _uow = uow;
           
        }

        [Route("ExibirRanking/{id:guid}")]
        public async Task<IActionResult> ExibirRanking(Guid Id)  
        {           
                       
            var rodada = await ObterRodada(Id);

            if (rodada == null)
            {
                //return View(new { success = false, message = "Ranking não encontrado." });
                return NotFound();
            }                      

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;

            var rankingRodadaRepository = _uow.GetRepository<RankingRodada>() as RankingRodadaRepository;
            var rankingRodadaViewModel = _mapper.Map<IEnumerable<RankingRodadaViewModel>>(await rankingRodadaRepository.ObterRankingDaRodadaEmOrdemDePontuacao(Id));
         
            // aqui pode ter uma view com todas as apostas geradas PARA CONFERÊNCIA
            return View(rankingRodadaViewModel);

        }


        private async Task<RodadaViewModel> ObterRodada(Guid id)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;

            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCampeonato(id));
            rodada.Campeonatos = _mapper.Map<IEnumerable<CampeonatoViewModel>>(await campeonatoRepository.ObterTodos());
            return rodada;
        }






    }

}
