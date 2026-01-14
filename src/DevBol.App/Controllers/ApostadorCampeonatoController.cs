using ApostasApp.Core.Infrastructure.Data.Repository.Apostas;
using AutoMapper;
using DevBol.Application;
using DevBol.Application.Interfaces.Campeonatos;
using DevBol.Domain.Interfaces;
using DevBol.Domain.Models.Apostadores;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Repository.Apostadores;
using DevBol.Infrastructure.Data.Repository.Apostas;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Rodadas;
using DevBol.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;


namespace DevBol.Presentation.Controllers
{
    public class ApostadorCampeonatoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;       
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;       

        public ApostadorCampeonatoController(IMapper mapper,
                                  IUnitOfWork uow,                                 
                                  IApostadorCampeonatoService apostadorCampeonatoService,                               
                                  INotificador notificador) : base(notificador)
        //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;           
            _apostadorCampeonatoService = apostadorCampeonatoService;           

        }

        public async Task<IActionResult> Index()
        {
            var campeonato = await ObterCampeonatoAtivo();

            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            return View(_mapper.Map<IEnumerable<ApostadorCampeonatoViewModel>>
                   (await apostadorCampeonatoRepository.ObterApostadoresDoCampeonato(campeonato.Id)));
        }

        [Route("novo-apostador-campeonato")]
        public async Task<IActionResult> Create()
        {
            var apostadorCampeonatoViewModel = await PopularApostadores(new ApostadorCampeonatoViewModel());

            var campeonato = await ObterCampeonatoAtivo();
           
            apostadorCampeonatoViewModel.Campeonato = campeonato;
            apostadorCampeonatoViewModel.CampeonatoId = campeonato.Id;
            
            return View(apostadorCampeonatoViewModel);
        }
       
        [Route("novo-apostador-campeonato")]
        [HttpPost]
        public async Task<IActionResult> Create(ApostadorCampeonatoViewModel apostadorCampeonatoViewModel)  
        {          

            if (!ModelState.IsValid) return View(apostadorCampeonatoViewModel);            

            //Aqui deverá ter um serviço com validação
            await _apostadorCampeonatoService.Adicionar(_mapper.Map<ApostadorCampeonato>(apostadorCampeonatoViewModel));

            if (!OperacaoValida()) return View(apostadorCampeonatoViewModel);

            //_uow.Commit();
            await _uow.SaveChanges();

            return RedirectToAction("Index");
        }

        //[Route("excluir-apostador-campeonato/{idCampeonato:guid}/{idApostador:guid}")]
        [Route("excluir-apostador-campeonato/{id:guid}")]
        //public async Task<IActionResult> Delete(Guid idCampeonato, Guid idApostadorCampeonato)
        public async Task<IActionResult> Delete(Guid id)
        {
            var apostadorCampeonatoViewModel = await ObterApostadorCampeonato(id);

            if (apostadorCampeonatoViewModel == null)
            {
                return NotFound();
            }
            var campeonato = await ObterCampeonato(apostadorCampeonatoViewModel.CampeonatoId); 
            
            //var apostador = await ObterApostadorDoCampeonato(idCampeonato, idApostador);
            
            apostadorCampeonatoViewModel.Campeonato = campeonato;
            //apostadorCampeonatoViewModel.Apostador = apostador.Apostador;

            return View(apostadorCampeonatoViewModel);
        }
                
        [Route("excluir-apostador-campeonato/{id:guid}")]
        [HttpPost, ActionName("Delete")]        
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {

            var apostadorCampeonatoViewModel = await ObterApostadorCampeonato(id);

            if (apostadorCampeonatoViewModel == null)
            {
                return NotFound();
            }

            //aqui deve ter o serviço também
            //await _apostadorCampeonatoService.RemoverEntity(_mapper.Map<ApostadorCampeonato>(apostadorCampeonatoViewModel));
            await _apostadorCampeonatoService.Remover(id);

            if (!OperacaoValida()) return View(apostadorCampeonatoViewModel);

            //_uow.Commit();
            //await _uow.SaveChanges();

            TempData["Sucesso"] = "Apostador desassociado do Campeonato com sucesso!";

            return RedirectToAction("Index");
        }

     
        [Route("listar-apostas-do-apostador")]
        public async Task<IActionResult> DeletarApostas()
        {
            //Aqui é EM TESTES .. depois tem que colocar pergunta se deseja realmente deletar
            //e não pode DELETAR se a RODADA já estiver com APOSTAS DE USUÁRIO (CORRENTE)

            var rodada = await ObterRodadaCorrente();

            var campeonatoId = rodada.Campeonato.Id;

            if (rodada == null)
            {
                return NotFound();
            }

            var apostaRepository = _uow.GetRepository<ApostaRodada>() as ApostaRodadaRepository;
            var apostas = await apostaRepository.ObterApostasDaRodada(rodada.Id);

            foreach (var a in apostas)
            {
                await apostaRepository.Remover(a.Id);
            }

            //salva a operação
            //_uow.Commit();           
            await _uow.SaveChanges();

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;

            var apostasDaRodada = await apostaRepository.ObterApostasDaRodada(rodada.Id);

            var apostaViewModel = _mapper.Map<IEnumerable<ApostaRodadaViewModel>>(await apostaRepository.ObterApostasDaRodada(rodada.Id));

            // aqui pode ter uma view sem as apostas deletadas PARA CONFERÊNCIA
            return View(apostaViewModel);

        }

        [Route("selecionar_apostador-campeonato_para_simular_aposta")]
        public async Task<IActionResult> SelecionarApostadorParaSimularAposta()
        {
            var campeonato = await ObterCampeonatoAtivo();

            if (campeonato == null)
            {
                return NotFound();
            }

            TempData["Campeonato"] = campeonato.Nome;

            var apostadorCampeonatoViewModel = await ObterApostadoresDoCampeonato(campeonato.Id);

            return View(apostadorCampeonatoViewModel);
        }


        [Route("simular-apostas-do-apostador-na-rodada/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> Apostar(Guid id)
        {
            var rodada = await ObterRodadaCorrente();

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;

            var apostador = await ObterApostador(id);

            TempData["Apostador"] = apostador.Usuario.Apelido;

            return View();            

        }

        [Route("consultar-apostas-do-apostador-na-rodada/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> ConsultarApostas(Guid id)
        {
            var rodada = await ObterRodadaCorrente();

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;

            var apostador = await ObterApostador(id);

            TempData["Apostador"] = apostador.Usuario.Apelido;

            return View();

        }

        //[Route("buscar-apostas-do-apostador-na-rodada/{id:guid}")]
        [Route("ApostadorCampeonato/ListarApostasDoApostadorNaRodada/{id}")]
        public async Task<IActionResult> ListarApostasDoApostadorNaRodada(Guid id)
        {
            var apostadorCampeonato = await ObterApostadorCampeonato(id);

            var rodada = await ObterRodadaCorrente();

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;
            TempData["Apostador"] = apostadorCampeonato.Apostador.Usuario.Apelido;
            
            TempData["IdApostadorCampeonato"] = apostadorCampeonato.Id;

            var apostaRepository = _uow.GetRepository<ApostaRodada>() as ApostaRodadaRepository;
            //Isso aqui deverá ser substituído por uma consulta em ApostasDaRodada, uma tabela
            //que deverá conter apenas DATAHORAAPOSTA, ID da RODADA, ID do APOSTADOR, ENVIADA, PONTOS
            var verifApostasNaRodada = await apostaRepository.ObterApostaSalvaDoApostadorNaRodada(rodada.Id, apostadorCampeonato.Id);

            if (verifApostasNaRodada.Enviada)
            {
                TempData["DATA_APOSTA"] = verifApostasNaRodada.DataHoraSubmissao.ToString();
                TempData["HORA_APOSTA"] = verifApostasNaRodada.DataHoraSubmissao.ToString();
                TempData["ENVIADA"] = "ENVIADA";
            }
            else
            {
                TempData["DATAHORA_APOSTA"] = "";
                TempData["ENVIADA"] = "AINDA NÃO ENVIADA";
            }

            return View(apostadorCampeonato);
        }

        //datatables com Get - Processamento no cliente
        //[HttpGet]
        public async Task<JsonResult> BuscaApostasDoApostadorNaRodada(Guid Id)
        {
            var rodada = await ObterRodadaCorrente();

            var palpiteRepository = _uow.GetRepository<Palpite>() as PalpiteRepository;
            var listaPalpites = await palpiteRepository.ObterPalpitesDoApostadorNaRodada(rodada.Id, Id);

            var data = listaPalpites.Select(palpite => new {                

                Id = palpite.Id,
                EquipeMandante = palpite.Jogo.EquipeCasa.Equipe.Nome,
                EscudoMandante = palpite.Jogo.EquipeCasa.Equipe.Escudo,
                PlacarMandante = palpite.PlacarApostaCasa == 0 && !palpite.ApostaRodada.Enviada ? "" : palpite.PlacarApostaCasa.ToString(), // Verifica se é zero
                EquipeVisitante = palpite.Jogo.EquipeVisitante.Equipe.Nome,
                EscudoVisitante = palpite.Jogo.EquipeVisitante.Equipe.Escudo,
                PlacarVisitante = palpite.PlacarApostaVisita == 0 && !palpite.ApostaRodada.Enviada ? "" : palpite.PlacarApostaVisita.ToString(), // Verifica se é zero
                Estadio = palpite.Jogo.Estadio.Nome, //+ " - " + aposta.Jogo.Estadio.Uf.Sigla,
                DataJogo = palpite.Jogo.DataJogo.ToShortDateString(),
                HoraJogo = palpite.Jogo.HoraJogo.ToString(),
                Enviada = palpite.ApostaRodada.Enviada,
                DataHoraAposta = palpite.ApostaRodada.DataHoraSubmissao == null ? "" : palpite.ApostaRodada.DataHoraSubmissao.ToString() //ToString("dd/MM/yyyy"), // Verifica se é nulo
              
            }).ToList();

            return Json(new { data });
        }

        //Ao clicar Salvar Apostas 
        /*
        [HttpPost]
        [Route("ApostadorCampeonato/SalvarApostas")]
        public async Task<JsonResult> SalvarApostas([FromBody] List<SalvarApostaViewModel> apostasViewModel)
        {
            //desativado provisoriamente
            if (apostasViewModel.IsNullOrEmpty()) // <-- Verifique se o resultado é vazio 
            {
                return Json(new { success = false, message = "Apostas não enviadas !!" });  // Ou outra ação apropriada, como redirecionar para uma página de erro
            }


            try
            {
                var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
                var apostas = _mapper.Map<List<Aposta>>(apostasViewModel);

                foreach (var aposta in apostas)
                {
                    var apostaDb = await apostaRepository.ObterPorId(aposta.Id);

                    if (apostaDb == null)
                    {
                        return Json(new { success = false, message = "Aposta não encontrada." });
                    }

                    apostaDb.PlacarApostaCasa = aposta.PlacarApostaCasa;
                    apostaDb.PlacarApostaVisita = aposta.PlacarApostaVisita;
                    apostaDb.DataHoraAposta = DateTime.Now;
                    apostaDb.Enviada = true;

                    apostaRepository.Atualizar(apostaDb);
                }

                //_uow.Commit();
                await _uow.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        */

        private async Task<CampeonatoViewModel> ObterCampeonato(Guid idCampeonato)
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonato(idCampeonato));
            return campeonato;
        }
        private async Task<RodadaViewModel> ObterRodadaCorrente()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCorrente());
            return rodada;
        }
                
        private async Task<ApostadorCampeonatoViewModel> ObterApostadorCampeonato(Guid idApostadorCampeonato)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var apostadorCampeonato = _mapper.Map<ApostadorCampeonatoViewModel>
                                  (await apostadorCampeonatoRepository.ObterApostadorCampeonato(idApostadorCampeonato));
            return apostadorCampeonato;
        }


        private async Task<CampeonatoViewModel> ObterCampeonatoAtivo()
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonatoAtivo());

            return campeonato;
        }

        private async Task<ApostadorViewModel> ObterApostador(Guid idApostadorCampeonato)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var apostadorCampeonato = _mapper.Map<ApostadorCampeonatoViewModel>(await apostadorCampeonatoRepository.ObterApostadorCampeonato(idApostadorCampeonato));

            var apostadorId = apostadorCampeonato.ApostadorId;

            var apostadorRepository = _uow.GetRepository<JogoFinalizadoEvent>() as ApostadorRepository;
            var apostador = _mapper.Map<ApostadorViewModel>(await apostadorRepository.ObterApostador(apostadorId));

            return apostador;
        }

        

        private async Task<ApostadorCampeonatoViewModel> PopularApostadores(ApostadorCampeonatoViewModel apostadorCampeonato)
        {
            var apostadorRepository = _uow.GetRepository<JogoFinalizadoEvent>() as ApostadorRepository;
            apostadorCampeonato.Apostadores = _mapper.Map<IEnumerable<ApostadorViewModel>>(await apostadorRepository.ObterTodos());
            
            return apostadorCampeonato;

        }

        private async Task<IEnumerable<ApostadorCampeonatoViewModel>> ObterApostadoresDoCampeonato(Guid id)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            return _mapper.Map<IEnumerable<ApostadorCampeonatoViewModel>>(await apostadorCampeonatoRepository.ObterApostadoresDoCampeonato(id));
        }



      




    }

}