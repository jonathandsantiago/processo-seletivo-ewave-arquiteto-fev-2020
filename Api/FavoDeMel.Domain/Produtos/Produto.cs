using FavoDeMel.Domain.Common;
using System;

namespace FavoDeMel.Domain.Produtos
{
    public class Produto : Entity<int>
    {
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }
}
