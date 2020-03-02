using FavoDeMel.Domain.Comandas;
using FavoDeMel.Domain.Produtos;
using FavoDeMel.Domain.Usuarios;
using FavoDeMel.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavoDeMel.Repository.Repositorys
{
    public class ComandaRepository : RepositoryBase<int, Comanda>, IComandaRepository
    {
        private DbSet<Produto> _dbSetProduto;
        private DbSet<ComandaPedido> _dbSetComandaPedido;
        private IUsuarioRepository _usuarioRepository;

        public ComandaRepository(RepositoryDbContext dbContext, IUsuarioRepository usuarioRepository) : base(dbContext)
        {
            _dbSetProduto = dbContext.Set<Produto>();
            _dbSetComandaPedido = dbContext.Set<ComandaPedido>();
            _usuarioRepository = usuarioRepository;
        }

        public IQueryable<Comanda> GetComandasAbertas()
        {
            return _dbSet.Where(c => c.Situacao == ComandaSituacao.Aberta)
                .Include(c => c.Garcom)
                .AsQueryable();
        }

        public async Task<Comanda> FecharConta(Comanda comanda)
        {
            comanda = await GetById(comanda.Id);

            comanda.FecharConta();
            _dbContext.Entry(comanda)
               .CurrentValues.SetValues(comanda);

            return comanda;
        }

        public override async Task<Comanda> Add(Comanda comanda)
        {
            IList<int> produtoIds = comanda.Pedidos.Select(c => c.Produto.Id).ToList();
            IList<Produto> produtos = await _dbSetProduto.AsQueryable()
                .Where(c => produtoIds.Contains(c.Id))
                .ToListAsync();

            comanda.Garcom = await _usuarioRepository.GetById(comanda.Garcom.Id);

            foreach (var pedido in comanda.Pedidos)
            {
                pedido.Produto = produtos.Where(c => c.Id == pedido.Produto.Id).FirstOrDefault();
                pedido.Comanda = comanda;
            }

            return _dbSet.Add(comanda).Entity;
        }

        public async Task<IList<Comanda>> TotasComandasAbertas()
        {
            return await _dbSet.Where(c => c.Situacao == ComandaSituacao.Aberta)
                .Include(c => c.Garcom)
                .Include(c => c.Pedidos)
                    .ThenInclude(c => c.Produto)
                .ToListAsync();
        }

        public async Task<Comanda> AlterarSituacaoPedido(int comandaId, ComandaPedidoSituacao situacao)
        {
            var pedidos = await _dbSetComandaPedido
                .Where(c => c.Comanda.Id == comandaId && c.Situacao != ComandaPedidoSituacao.Cancelado)
                .ToListAsync();

            foreach (var pedido in pedidos)
            {
                pedido.Situacao = situacao;
                _dbContext.Update(pedido);
            }

            await Commit();
            return await _dbSet.Where(c => c.Id == comandaId)
               .Include(c => c.Garcom)
               .Include(c => c.Pedidos)
                   .ThenInclude(c => c.Produto)
               .FirstOrDefaultAsync();
        }
    }
}
