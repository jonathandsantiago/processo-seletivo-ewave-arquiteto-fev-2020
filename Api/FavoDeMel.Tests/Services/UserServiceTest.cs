using FavoDeMel.Domain.Common;
using FavoDeMel.Domain.Usuarios;
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
    public class UserServiceTest
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
            IUsuarioService service = serviceProvider.GetRequiredService<IUsuarioService>();
            TesteDeCrudIntegracao(service).Wait();
        }

        [Test]
        public void TesteValidacaoUsuarioJaCadastrado()
        {
            TesteValidacaoUsuarioJaCadastradoAsync().Wait();
        }

        private async Task TesteValidacaoUsuarioJaCadastradoAsync()
        {
            IUsuarioService service = serviceProvider.GetRequiredService<IUsuarioService>();
            Usuario usuarioNovo = null;

            try
            {
                Usuario usuario = new Usuario();
                usuario.Nome = $"Usuário de teste2";
                usuario.Login = "teste2";
                usuario.Password = DomainHelper.CalculateMD5Hash("teste2");
                usuario.Setor = UsuarioSetor.Cozinha;
                usuarioNovo = await service.Add(usuario);
                service.Result.Any().Should().BeFalse();

                await CadastrarNovoUsuarioCoMesmoLogin();
            }
            finally
            {
                if (usuarioNovo != null)
                {
                    bool resultadoExclusao = await service.Delete(usuarioNovo.Id);
                    resultadoExclusao.Should().BeTrue();
                }
            }
        }

        private async Task CadastrarNovoUsuarioCoMesmoLogin()
        {
            IUsuarioService service = serviceProvider.GetRequiredService<IUsuarioService>();
            Usuario usuario = new Usuario();
            usuario.Nome = $"Usuário de teste2";
            usuario.Login = "teste2";
            usuario.Password = DomainHelper.CalculateMD5Hash("teste2");
            usuario.Setor = UsuarioSetor.Cozinha;
            await service.Add(usuario);
            service.Result.Any().Should().BeTrue();
        }

        private async Task TesteDeCrudIntegracao(IUsuarioService service)
        {
            Usuario usuarioNovo = null;

            try
            {
                Usuario usuario = new Usuario();
                usuario.Nome = $"Usuário de teste";
                usuario.Login = "teste";
                usuario.Password = DomainHelper.CalculateMD5Hash("teste");
                usuario.Setor = UsuarioSetor.Cozinha;
                usuarioNovo = await service.Add(usuario);
                service.Result.Any().Should().BeFalse();

                usuarioNovo.Should().NotBeNull();
                usuarioNovo.Id.Should().NotBe(0);
                usuarioNovo.Nome.Should().Be(usuario.Nome);

                usuarioNovo.Nome = $"{usuarioNovo.Nome} - Editado";
                bool resultadoEdicao = await service.Edity(usuarioNovo);
                resultadoEdicao.Should().BeTrue();

                Usuario usuarioLogin = await service.GetByLoginPassword(usuario.Login, usuario.Password);
                usuarioLogin.Should().NotBeNull();

                IEnumerable<Usuario> entidades = await service.GetAll();
                entidades.Any().Should().BeTrue();

                Usuario usuarioById = await service.GetById(usuarioNovo.Id);
                usuarioById.Should().NotBeNull();
                usuarioById.Nome.Should().Be(usuario.Nome);

                bool exists = service.Exists(usuarioNovo.Id);
                exists.Should().BeTrue();
            }
            finally
            {
                if (usuarioNovo != null)
                {
                    bool resultadoExclusao = await service.Delete(usuarioNovo.Id);
                    resultadoExclusao.Should().BeTrue();
                }
            }
        }
    }
}