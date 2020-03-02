using AutoMapper;
using FavoDeMel.Api.Dtos;
using FavoDeMel.Domain.Common;
using FavoDeMel.IoC.Auth;
using FavoDeMel.Service.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FavoDeMel.Api.Controllers.Common
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public abstract class ControllerBase<TEntity, TId, TDto, TAppService> : ControllerBase
      where TEntity : Entity<TId>
      where TDto : DtoBase<TId>
      where TAppService : IServiceBase<TId, TEntity>
    {
        protected readonly TAppService _appService;

        protected readonly IBus _bus;

        protected virtual Guid UserId
        {
            get
            {
                Claim claim = GetClaim(ClaimName.UserId);
                return claim == null ? Guid.Empty : new Guid(claim.Value);
            }
        }

        protected virtual string UserName
        {
            get
            {
                return GetClaim(ClaimName.UserName)?.Value;
            }
        }

        public ControllerBase(TAppService appService)
        {
            _appService = appService;
        }

        public ControllerBase(TAppService appService, IBus bus)
          : this(appService)
        {
            _bus = bus;
        }

        protected virtual async Task<IActionResult> ExecuteAction(ActionType actionType, TDto entity)
        {
            try
            {
                return actionType switch
                {
                    ActionType.Post => await ExecuteInsert(entity),
                    ActionType.Put => await ExecuteEdit(entity),
                    ActionType.Delete => await ExecuteDelete(entity.Id),
                    _ => throw new NotImplementedException(),
                };
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        protected virtual async Task<IActionResult> ExecuteInsert(TDto dto)
        {
            TEntity entity = OnSave(dto);
            entity = await _appService.Add(entity);

            if (entity == null || entity.Id.Equals(dto.Id))
            {
                if (!_appService.Result.Any())
                {
                    return BadRequest("Ocorreu um erro ao realizar o cadastro, tente novamente mais tarde!");
                }

                return BadRequest(string.Join(Environment.NewLine, _appService.Result));
            }

            dto.Id = entity.Id;

            OnAffertSave(entity);
            return Ok(entity);
        }

        protected virtual void OnAffertSave(TEntity entity)
        { }

        protected virtual TEntity OnSave(TDto dto)
        {
            return Mapper.Map<TEntity>(dto);
        }

        protected virtual async Task<IActionResult> ExecuteEdit(TDto dto)
        {
            var entity = await _appService.GetById(dto.Id);

            if (!await _appService.Edity(OnUpdate(dto, entity)))
            {
                return BadRequest();
            }

            return Ok(true);
        }

        protected virtual async Task<IActionResult> ExecuteDelete(TId id)
        {
            if (!await _appService.Delete(id))
            {
                return BadRequest();
            }

            return Ok(true);
        }

        protected virtual TEntity OnUpdate(TDto dto, TEntity entity)
        {
            return Mapper.Map(dto, entity);
        }

        protected virtual IActionResult Ok<TResultDto, TResult>(IList<TResult> value)
            where TResultDto : DtoBase<TId>
            where TResult : Entity<TId>
        {
            return Ok(Mapper.Map<IList<TResultDto>>(value));
        }

        protected virtual IActionResult Ok<TResultDto, TResult>(TResult value)
            where TResultDto : DtoBase<TId>
            where TResult : Entity<TId>
        {
            return Ok(Mapper.Map<TResultDto>(value));
        }

        protected virtual IActionResult Ok(IList<TEntity> entitys)
        {
            return Ok(Mapper.Map<IList<TDto>>(entitys));
        }

        protected virtual IActionResult Ok(TEntity entity)
        {
            return Ok(Mapper.Map<TDto>(entity));
        }

        protected virtual IActionResult Error(Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

        protected virtual IActionResult BadRequest(string message = null)
        {
            ModelState.AddModelError("message", message);

            foreach (var error in _appService.Result)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return BadRequest(ModelState);
        }

        protected virtual Claim GetClaim(string claim)
        {
            if (User == null)
            {
                return null;
            }

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            return identity?.Claims.FirstOrDefault(c => c.Type == claim);
        }

        protected virtual ClaimsIdentity GetClaimsIdentity()
        {
            return (ClaimsIdentity)User.Identity;
        }

        protected virtual string GetMessageValidator()
        {
            return string.Join(Environment.NewLine, _appService.Result.Select(c => c));
        }

        protected virtual async Task SendMessageComanda<TMessage>(TMessage result)
            where TMessage : class
        {
            await _bus.Send<IMessage>(new { Comanda = JsonConvert.SerializeObject(result) }) ;
        }

        protected virtual async Task SendMessage(string messsage)
        {
            await _bus.Send<IMessage>(new { Notificacao = messsage });
        }
    }
}