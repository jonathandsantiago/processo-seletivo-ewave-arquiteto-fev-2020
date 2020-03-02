using FavoDeMel.Domain.Comandas;
using FavoDeMel.Domain.Produtos;
using FavoDeMel.Domain.Usuarios;
using FavoDeMel.Repository.Repositorys;
using Microsoft.Extensions.DependencyInjection;

namespace FavoDeMel.IoC
{
    public static class RepositoryInject
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IComandaRepository, ComandaRepository>();
            return services;
        }
    }
}
