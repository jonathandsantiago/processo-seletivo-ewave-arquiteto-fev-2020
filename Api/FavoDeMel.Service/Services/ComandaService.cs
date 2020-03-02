using FavoDeMel.Domain.Comandas;
using FavoDeMel.Service.Common;
using FavoDeMel.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavoDeMel.Service.Services
{
    public class ComandaService : ServiceBase<int, Comanda, IComandaRepository>, IComandaService
    {
        public ComandaService(IComandaRepository repository) : base(repository)
        {
            _validator = new ComandaValidator(repository);
        }

        public IQueryable<Comanda> ComandasAbertas()
        {
            return _repository.GetComandasAbertas();
        }

        public async Task<Comanda> AlterarSituacaoPedido(int comandaId, ComandaPedidoSituacao situacao)
        {
            return await _repository.AlterarSituacaoPedido(comandaId, situacao);
        }

        public async Task<IList<Comanda>> TotasComandasAbertas()
        {
            return await _repository.TotasComandasAbertas();
        }
    }
}
