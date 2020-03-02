using FavoDeMel.Domain.Usuarios;
using FavoDeMel.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FavoDeMel.Repository.Repositorys
{
    public class UsuarioRepository : RepositoryBase<int, Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(RepositoryDbContext dbContext) : base(dbContext)
        { }

        public async Task<bool> ExistsLogin(int id, string login)
        {
            return await _dbSet.AnyAsync(c => c.Id != id && c.Login == login);
        }

        public async Task<Usuario> GetByLoginPassword(string login, string password)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Login == login && c.Password == password);
        }
    }
}
