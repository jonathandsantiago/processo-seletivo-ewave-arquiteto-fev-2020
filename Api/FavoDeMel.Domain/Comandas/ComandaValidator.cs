using FavoDeMel.Domain.Common;
using System.Linq;
using System.Threading.Tasks;

namespace FavoDeMel.Domain.Comandas
{
    public class ComandaValidator : ValidatorBase<int, Comanda, IComandaRepository>
    {
        public ComandaValidator(IComandaRepository repository) :
            base(repository)
        { }

        public override async Task<bool> Validate(Comanda entity)
        {
            if (entity == null)
            {
                AddMensagem("Comanda inválida!");
            }

            if (!entity.Pedidos.Any())
            {
                AddMensagem("Pedido é obrigatório!");
            }
            else if (entity.Pedidos.Any(c => c.Produto == null || c.Produto.Id == 0))
            {
                AddMensagem("Produto é obrigatório!");
            }
            else if (entity.Pedidos.Any(c => c.Quantidade == 0))
            {
                AddMensagem("Quantidade é obrigatório!");
            }

            return await Task.Run(() => ResultIsValid);
        }
    }
}
