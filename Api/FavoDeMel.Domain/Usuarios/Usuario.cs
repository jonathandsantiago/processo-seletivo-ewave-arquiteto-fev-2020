﻿using FavoDeMel.Domain.Common;

namespace FavoDeMel.Domain.Usuarios
{
    public class Usuario : Entity<int>
    {
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UsuarioSetor Setor { get; set; }
    }
}
