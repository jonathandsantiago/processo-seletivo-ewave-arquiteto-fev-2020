using FavoDeMel.Domain.Common;
using System.Threading.Tasks;

namespace FavoDeMel.Domain.Produtos
{
    public class ProdutoValidator : ValidatorBase<int, Produto, IProdutoRepository>
    {
        public ProdutoValidator(IProdutoRepository repository) :
            base(repository)
        { }

        public override async Task<bool> Validate(Produto entity)
        {
            if (entity == null)
            {
                AddMensagem("Produto inválido!");
            }

            if (string.IsNullOrEmpty(entity.Nome))
            {
                AddMensagem("Nome é obrigatório!");
            }

            if (entity.Preco <= 0)
            {
                AddMensagem("Preço inválido!");
            }

            if (await _repository.ExistsProduto(entity.Id, entity.Nome))
            {
                AddMensagem("Login já cadastrado!");
            }

            return ResultIsValid;
        }
    }
}
