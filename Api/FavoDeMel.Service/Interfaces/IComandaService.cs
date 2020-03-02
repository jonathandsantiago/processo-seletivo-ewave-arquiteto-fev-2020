using FavoDeMel.Domain.Comandas;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavoDeMel.Service.Interfaces
{
    public interface IComandaService : IServiceBase<int, Comanda>
    {
        IQueryable<Comanda> ComandasAbertas();
        Task<IList<Comanda>> TotasComandasAbertas();
        Task<Comanda> AlterarSituacaoPedido(int comandaId, ComandaPedidoSituacao situacao);
    }
}
