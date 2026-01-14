using DevBol.Application.Interfaces.Jogos;
using DevBol.Domain.Models.Jogos;
using DevBol.Application.Base;
using DevBol.Domain.Interfaces;
using DevBol.Infrastructure.Data.Interfaces.Jogos;


namespace DevBol.Application.Services.Jogos
{
    public class JogoService : BaseService, IJogoService
    {
        private readonly IJogoRepository _jogoRepository;
       

        public JogoService(IJogoRepository jogoRepository,
                                 INotificador notificador,
                                 IUnitOfWork uow  ) : base(notificador, uow)
        {
            _jogoRepository = jogoRepository;
           
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(Jogo jogo)
        {
            if (!ExecutarValidacao(new JogoValidation(), jogo))
                return;           

              if (_jogoRepository.Buscar(j => j.EquipeCasaId == jogo.EquipeCasaId 
                && j.EquipeVisitanteId == jogo.EquipeVisitanteId).Result.Any())
              {
                  Notificar("Este JOGO já está cadastrado.");
                  return;
              }
                            

            await _jogoRepository.Adicionar(jogo);
        }

        public async Task Atualizar(Jogo jogo)
        {
            if (!ExecutarValidacao(new JogoValidation(), jogo)) return;
            try
            {
                await _jogoRepository.Atualizar(jogo);

                // 2. DISPARA PARA O WORKER (Assíncrono)
                // Aqui o Controller é liberado na hora!
                //await _queue.EscreverJogoAsync(jogo.RodadaId);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($">>> [FILA] Rodada {jogo.RodadaId} enviada para a fila com sucesso!");
                Console.ResetColor();


            }
            catch (Exception ex)
            {
                Notificar($"Erro ao atualizar o jogo no banco de dados: {ex.Message}");
                // Logar o erro aqui também
                return;
            }
        }

        public async Task Remover(Guid id)
        {
            //ver o que entra aqui - Não pode excluir um JOGO que tem APOSTAS ..


            /*var rodada = await _jogoRepository.ObterJogosDaRodada(id);

            if (rodada.Any())
            {
                Notificar("O RODADA possui JOGOS associados ! Não pode ser excluída ");
                return;
            }
*/
            await _jogoRepository.Remover(id);
        }

      /*  public void Dispose()
        {
            _jogoRepository?.Dispose();
        }*/
    }
}