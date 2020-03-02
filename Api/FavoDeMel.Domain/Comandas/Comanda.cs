using FavoDeMel.Domain.Common;
using FavoDeMel.Domain.Produtos;
using FavoDeMel.Domain.Usuarios;
using System.Collections.Generic;
using System.Linq;

namespace FavoDeMel.Domain.Comandas
{
    public class Comanda : Entity<int>
    {
        public Usuario Garcom { get; set; }
        public IList<ComandaPedido> Pedidos { get; set; }
        public decimal TotalAPagar { get; set; }
        public decimal GorjetaGarcom { get; set; }
        public ComandaSituacao Situacao { get; set; }

        public Comanda()
        {
            Pedidos = new List<ComandaPedido>();
        }

        public void FecharConta()
        {
            TotalAPagar = Pedidos.Sum(c => c.Quantidade * c.Produto.Preco);
            GorjetaGarcom = (TotalAPagar / 100) * 10;
            Situacao = ComandaSituacao.Fechada;
        }

        public void AdicionarPedido(Produto produto, int quantidade)
        {
            Pedidos.Add(new ComandaPedido(this, produto, quantidade));
        }
    }
}