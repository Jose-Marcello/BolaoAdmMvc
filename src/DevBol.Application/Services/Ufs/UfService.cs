using DevBol.Application.Interfaces.Ufs;
using DevBol.Domain.Models.Ufs;
using DevBol.Domain.Interfaces;
using DevBol.Application.Base;
using DevBol.Domain.Interfaces.Ufs;

namespace DevBol.Application.Services.Ufs
{
    public class UfService : BaseService, IUfService
    {
        private readonly IUfRepository _ufRepository;
        
        
        public UfService(IUfRepository ufRepository,
                                 INotificador notificador,
                                 IUnitOfWork uow) : base(notificador, uow)
        {
            _ufRepository = ufRepository;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(Uf uf)
        {
            if (!ExecutarValidacao(new UfValidation(), uf))
                return;           

              if (_ufRepository.Buscar(e => e.Nome == uf.Nome).Result.Any())
              {
                  Notificar("Já existe uma UF com este NOME infomado.");
                  return;
              }

            await _ufRepository.Adicionar(uf);
        }

        public async Task Atualizar(Uf uf)
        {
            if (!ExecutarValidacao(new UfValidation(), uf)) return;
                        

            await _ufRepository.Atualizar(uf);
        }

        public async Task Remover(Guid id)
        {         
            await _ufRepository.Remover(id);
        }

       /* public void Dispose()
        {
            _ufRepository?.Dispose();
        }*/
    }
}