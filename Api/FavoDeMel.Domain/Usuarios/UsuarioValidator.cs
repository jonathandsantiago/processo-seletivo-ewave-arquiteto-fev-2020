using FavoDeMel.Domain.Common;
using System.Threading.Tasks;

namespace FavoDeMel.Domain.Usuarios
{
    public class UsuarioValidator : ValidatorBase<int, Usuario, IUsuarioRepository>
    {
        public UsuarioValidator(IUsuarioRepository repository) :
            base(repository)
        { }

        public override async Task<bool> Validate(Usuario entity)
        {
            if (await _repository.ExistsLogin(entity.Id, entity.Login))
            {
                AddMensagem("Login já cadastrado");
            }

            return ResultIsValid;
        }
    }
}