using FavoDeMel.Domain.Comandas;
using FavoDeMel.Domain.Produtos;
using FavoDeMel.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace FavoDeMel.Repository.Common
{
    public class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext()
        { }

        public RepositoryDbContext(DbContextOptions<RepositoryDbContext> options)
            : base(options)
        { }

        public DbSet<Produto> Produto { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Comanda> Comanda { get; set; }
        public DbSet<ComandaPedido> ComandaPedido { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Produto>();
            builder.Entity<Usuario>();
            builder.Entity<Comanda>()
                .HasMany(c => c.Pedidos);
            builder.Entity<ComandaPedido>();
        }
    }
}
