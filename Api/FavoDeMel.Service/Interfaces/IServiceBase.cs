﻿using FavoDeMel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FavoDeMel.Service.Interfaces
{
    public interface IServiceBase<TId, TEntity> : IDisposable
           where TEntity : Entity<TId>
    {
        Task<TEntity> GetById(TId id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Add(TEntity entity);
        Task<bool> Edity(TEntity entity);
        Task<bool> Delete(TId id);
        IList<string> Result { get; }
        bool Exists(TId id);
    }
}
