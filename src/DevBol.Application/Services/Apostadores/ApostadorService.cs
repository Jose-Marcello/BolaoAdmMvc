using DevBol.Application.Interfaces.Apostadores;
using DevBol.Domain.Interfaces;
using DevBol.Application.Base;
using DevBol.Infrastructure.Data.Interfaces.Apostadores;
using DevBol.Domain.Models.Apostadores;
using DevBol.Domain.Validations;

namespace DevBol.Application.Services.Apostadors
{
    public class ApostadorService : BaseService, IApostadorService
    {
        private readonly IApostadorRepository _apostadorRepository;

        public ApostadorService(IApostadorRepository apostadorRepository,
                                 INotificador notificador, 
                                 IUnitOfWork uow) : base(notificador,uow)
        {
            _apostadorRepository = apostadorRepository;
        }

        public async Task Adicionar(JogoFinalizadoEvent apostador)
        {
            if (!ExecutarValidacao(new ApostadorValidation(), apostador))
            {                 
                return;            
            }
                
           /* //trocar para CPF 
            if (_apostadorRepository.Buscar(a => a.CPF == apostador.CPF).Result.Any())
            {
                Notificar("Já existe um apostador com este CPF informado.");
                return;
            }
            if (_apostadorRepository.Buscar(a => a.Nome == apostador.Nome).Result.Any())
            {
                Notificar("Já existe um apostador com este NOME informado.");
                return;
            }*/


            await _apostadorRepository.Adicionar(apostador);
        }

        public async Task Atualizar(JogoFinalizadoEvent apostador)
        {
            if (!ExecutarValidacao(new ApostadorValidation(), apostador)) return;

           /* //tem que checar CPF 
            if (_apostadorRepository.Buscar(f => f.Nome == apostador.Nome && f.Id != apostador.Id).Result.Any())
            {
                Notificar("Já existe um apostador com este NOME informado.");
                return;
            }*/

            await _apostadorRepository.Atualizar(apostador);
        }

        public async Task Remover(Guid id)
        {
            //var apostador = await _apostadorRepository.ObterRodadasDoApostador(id);

            //if (apostador.Rodadas.Any())
            //{
            //    Notificar("O apostador possui RODADAS associadas !");
            //    return;
            //}

            await _apostadorRepository.Remover(id);
        }

        public async Task RemoverEntity(JogoFinalizadoEvent apostador)
        {
            //var apostador = await _apostadorRepository.ObterRodadasDoApostador(id);

            //if (apostador.Rodadas.Any())
            //{
            //    Notificar("O apostador possui RODADAS associadas !");
            //    return;
            //}

            await _apostadorRepository.RemoverEntity(apostador);
        }

        /*public void Dispose()
        {
            _apostadorRepository?.Dispose();
        }*/
    }
}