using FavoDeMel.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavoDeMel.Domain.Common
{
    public abstract class ValidatorBase<TId, TEntity, TRepository>
        where TEntity : Entity<TId>
        where TRepository : IRepositoryBase<TId, TEntity>
    {
        public virtual IList<string> Messages { get; protected set; }
        public virtual bool ResultIsValid { get { return !Messages.Any(); } }
        public virtual TRepository _repository { get; protected set; }

        public ValidatorBase()
        {
            Messages = new List<string>();
        }

        public ValidatorBase(TRepository repository)
            : this()
        {
            _repository = repository;
        }

        public abstract Task<bool> Validate(TEntity entity);

        public virtual async Task<bool> AllowsRemove(TEntity entity)
        {
            return await Task.Run(() => ResultIsValid);
        }

        protected virtual void AddMensagem(string message)
        {
            Messages.Add(message);
        }
    }
}
