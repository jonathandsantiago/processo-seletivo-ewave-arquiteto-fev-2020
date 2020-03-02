using System.Threading.Tasks;
using FavoDeMel.Domain.Interfaces;

namespace FavoDeMel.Domain.Produtos
{
    public interface IProdutoRepository : IRepositoryBase<int, Produto>
    {
        Task<bool> ExistsProduto(int id, string nome);
    }
}
