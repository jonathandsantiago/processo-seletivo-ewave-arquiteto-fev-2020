using FavoDeMel.Domain.Comandas;
using FavoDeMel.Domain.Common;
using FavoDeMel.Domain.Produtos;
using FavoDeMel.Domain.Usuarios;
using FavoDeMel.IoC;
using FavoDeMel.Repository.Common;
using FavoDeMel.Service.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavoDeMel.Tests.Services
{
    public class ComandaServiceTest
    {
        private ServiceProvider serviceProvider;
        private IConfigurationRoot _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = Startup.Configuration;
            var services = new ServiceCollection();
            services.AddSingleton(_configuration);
            services.AddOptions();

            services.AddDbContext<RepositoryDbContext>(c => c.UseSqlServer(_configuration.GetConnectionString("connectionName")));
            services.AddServices();
            services.AddRepository();


            serviceProvider = services.BuildServiceProvider();
        }
        [Test]
        public void TesteDeCrudIntegracao()
        {
            TesteDeCrudIntegracaoAsync().Wait();
        }

        private async Task TesteDeCrudIntegracaoAsync()
        {
            IComandaService service = serviceProvider.GetRequiredService<IComandaService>();
            IUsuarioService usuarioService = serviceProvider.GetRequiredService<IUsuarioService>();
            IProdutoService produtoService = serviceProvider.GetRequiredService<IProdutoService>();

            Comanda comandaNovo = null;
            Usuario usuario = null;
            Produto produto = null;
            Produto produto2 = null;

            try
            {
                usuario = await ObterUsuario(usuarioService);
                produto = await ObterProduto(produtoService, $"Produto pedido");
                produto2 = await ObterProduto(produtoService, $"Produto pedido 2");

                Comanda comanda = new Comanda();
                comanda.Garcom = usuario;
                comanda.AdicionarPedido(produto, 1);
                comandaNovo = await service.Add(comanda);
                service.Result.Any().Should().BeFalse();

                Comanda comandaPedidoEmPreparo = await service.AlterarSituacaoPedido(comandaNovo.Id, ComandaPedidoSituacao.Preparando);
                comandaPedidoEmPreparo.Pedidos.All(c => c.Situacao == ComandaPedidoSituacao.Preparando).Should().BeTrue();

                Comanda comandaPedidoPronto = await service.AlterarSituacaoPedido(comandaNovo.Id, ComandaPedidoSituacao.Pronto);
                comandaPedidoPronto.Pedidos.All(c => c.Situacao == ComandaPedidoSituacao.Pronto).Should().BeTrue();

                Comanda comandaPedidoCancelado = await service.AlterarSituacaoPedido(comandaNovo.Id, ComandaPedidoSituacao.Cancelado);
                comandaPedidoCancelado.Pedidos.All(c => c.Situacao == ComandaPedidoSituacao.Cancelado).Should().BeTrue();

                IList<Comanda> comandasAbertas = await service.TotasComandasAbertas();
                comandasAbertas.Any().Should().BeTrue();

                IQueryable<Comanda> comandasAbertasQueryable = service.ComandasAbertas();
                comandasAbertasQueryable.Any().Should().BeTrue();

                comandaNovo = await comandasAbertasQueryable.Where(c => c.Id == comandaNovo.Id)
                    .Include(c => c.Garcom)
                    .Include(c => c.Pedidos)
                        .ThenInclude(c => c.Produto)
                    .FirstOrDefaultAsync();

                comandaNovo.Should().NotBeNull();
                comandaNovo.Id.Should().NotBe(0);
                comandaNovo.Garcom.Nome.Should().Be(usuario.Nome);
                comandaNovo.AdicionarPedido(produto2, 1);
                comandaNovo.FecharConta();
                comandaNovo.Situacao.Should().Be(ComandaSituacao.Fechada);
                comandaNovo.TotalAPagar.Should().Be(20);
                comandaNovo.GorjetaGarcom.Should().Be(2.0M);

                bool resultadoEdicao = await service.Edity(comandaNovo);
                resultadoEdicao.Should().BeTrue();

                IEnumerable<Comanda> entidades = await service.GetAll();
                entidades.Any().Should().BeTrue();

                Comanda comandaById = await service.GetById(comandaNovo.Id);
                comandaById.Should().NotBeNull();

                bool exists = service.Exists(comandaNovo.Id);
                exists.Should().BeTrue();
            }
            finally
            {
                if (comandaNovo != null)
                {
                    bool resultadoExclusao = await service.Delete(comandaNovo.Id);
                    resultadoExclusao.Should().BeTrue();
                }

                if (usuario != null)
                {
                    bool resultadoExclusao = await usuarioService.Delete(usuario.Id);
                    resultadoExclusao.Should().BeTrue();
                }

                if (produto != null)
                {
                    bool resultadoExclusao = await produtoService.Delete(produto.Id);
                    resultadoExclusao.Should().BeTrue();
                }

                if (produto2 != null)
                {
                    bool resultadoExclusao = await produtoService.Delete(produto2.Id);
                    resultadoExclusao.Should().BeTrue();
                }
            }
        }

        private async Task<Usuario> ObterUsuario(IUsuarioService service)
        {
            Usuario usuario = new Usuario();
            usuario.Nome = $"Usuário de teste";
            usuario.Login = "teste";
            usuario.Password = DomainHelper.CalculateMD5Hash("teste");
            usuario.Setor = UsuarioSetor.Cozinha;
            return await service.Add(usuario);
        }

        private async Task<Produto> ObterProduto(IProdutoService service, string nome)
        {
            Produto produto = new Produto();
            produto.Nome = nome;
            produto.Preco = 10;
            return await service.Add(produto);
        }
    }
}