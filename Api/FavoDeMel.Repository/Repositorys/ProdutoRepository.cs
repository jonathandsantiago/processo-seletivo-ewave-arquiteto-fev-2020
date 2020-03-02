using FavoDeMel.Domain.Produtos;
using FavoDeMel.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FavoDeMel.Repository.Repositorys
{
    public class ProdutoRepository : RepositoryBase<int, Produto>, IProdutoRepository
    {
        public ProdutoRepository(RepositoryDbContext dbContext) : base(dbContext)
        { }

        public async Task<bool> ExistsProduto(int id, string nome)
        {
            return await _dbSet.AnyAsync(c => c.Id != id && c.Nome == nome);
        }
    }
}
