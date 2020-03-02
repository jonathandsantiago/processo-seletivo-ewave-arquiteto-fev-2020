using FavoDeMel.Domain.Produtos;
using FavoDeMel.IoC;
using FavoDeMel.Repository.Common;
using FavoDeMel.Service.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavoDeMel.Tests
{
    public class ProdutoServiceTest
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
            IProdutoService service = serviceProvider.GetRequiredService<IProdutoService>();
            TesteDeCrudIntegracao(service).Wait();
        }

        [Test]
        public void TesteValidacaoProdutoJaCadastrado()
        {
            TesteValidacaoProdutoJaCadastradoAsync().Wait();
        }

        private async Task TesteValidacaoProdutoJaCadastradoAsync()
        {
            IProdutoService service = serviceProvider.GetRequiredService<IProdutoService>();
            Produto produtoNovo = null;

            try
            {
                Produto produto = new Produto();
                produto.Nome = $"Produto de teste2";
                produto.Preco = 10;
                produtoNovo = await service.Add(produto);
                service.Result.Any().Should().BeFalse();
                await CadastrarNovoProdutoCoMesmoLogin();
            }
            finally
            {
                if (produtoNovo != null)
                {
                    bool resultadoExclusao = await service.Delete(produtoNovo.Id);
                    resultadoExclusao.Should().BeTrue();
                }
            }
        }

        private async Task CadastrarNovoProdutoCoMesmoLogin()
        {
            IProdutoService service = serviceProvider.GetRequiredService<IProdutoService>();
            Produto produto = new Produto();
            produto.Nome = $"Produto de teste2";
            produto.Preco = 10;
            await service.Add(produto);
            service.Result.Any().Should().BeTrue();
        }

        private async Task TesteDeCrudIntegracao(IProdutoService service)
        {
            Produto produtoNovo = null;

            try
            {
                Produto produto = new Produto();
                produto.Nome = $"Produto de teste";
                produto.Preco = 10;
                produtoNovo = await service.Add(produto);
                service.Result.Any().Should().BeFalse();

                produtoNovo.Should().NotBeNull();
                produtoNovo.Id.Should().NotBe(0);
                produtoNovo.Nome.Should().Be(produto.Nome);

                produtoNovo.Nome = $"{produtoNovo.Nome} - Editado";
                bool resultadoEdicao = await service.Edity(produtoNovo);
                resultadoEdicao.Should().BeTrue();

                IEnumerable<Produto> produtos = await service.GetAll();
                produtos.Any().Should().BeTrue();

                Produto produtoById = await service.GetById(produtoNovo.Id);
                produtoById.Should().NotBeNull();
                produtoById.Nome.Should().Be(produto.Nome);

                bool exists = service.Exists(produtoNovo.Id);
                exists.Should().BeTrue();
            }
            finally
            {
                if (produtoNovo != null)
                {
                    bool resultadoExclusao = await service.Delete(produtoNovo.Id);
                    resultadoExclusao.Should().BeTrue();
                }
            }
        }
    }
}