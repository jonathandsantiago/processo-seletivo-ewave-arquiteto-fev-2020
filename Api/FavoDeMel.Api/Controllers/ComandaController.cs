using AutoMapper;
using FavoDeMel.Api.Controllers.Common;
using FavoDeMel.Api.Dtos;
using FavoDeMel.Domain.Comandas;
using FavoDeMel.Service.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FavoDeMel.Api.Controllers
{
    public class ComandaController : ControllerBase<Comanda, int, ComandaDto, IComandaService>
    {
        public ComandaController(IComandaService service, IBus bus)
            : base(service, bus)
        { }

        [HttpPost()]
        public async Task<IActionResult> Cadatrar([FromBody]ComandaDto comandaDto)
        {
            try
            {
                return await ExecuteAction(ActionType.Post, comandaDto);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        protected override void OnAffertSave(Comanda entity)
        {
            SendMessage(Mapper.Map<ComandaDto>(entity)).Wait();
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody]ComandaDto comandaDto)
        {
            try
            {
                return await ExecuteAction(ActionType.Put, comandaDto);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPut("{comandaId}")]
        public async Task<IActionResult> AlterarSituacaoPedido([FromRoute]int comandaId, [FromBody] ComandaPedidoSituacao situacao)
        {
            try
            {
                return Ok(await _appService.AlterarSituacaoPedido(comandaId, situacao));
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Responsável por obter todas comandas abertas
        /// </summary>
        [HttpGet()]
        public virtual async Task<IActionResult> TotasComandasAbertas()
        {
            try
            {
                return Ok(await _appService.TotasComandasAbertas());
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Responsável por obter comanda pelo id
        /// </summary>
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Obter(int id)
        {
            try
            {
                return Ok(await _appService.GetById(id));
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Responsável por obter todos as comandas em aberta
        /// </summary>
        /// <returns>Lista das comandas em amberto</returns>
        [HttpGet("{pageNumber}/{pageSize}")]
        public virtual async Task<IActionResult> ComandasAbertas(int pageNumber, int pageSize)
        {
            try
            {
                var source = _appService.ComandasAbertas();
                int skipRows = (pageNumber - 1) * pageSize;
                int count = await source.CountAsync();
                var items = await source.Skip(skipRows <= 0 ? 0 : skipRows).Take(pageSize).ToListAsync();

                return Ok(new
                {
                    Items = items,
                    Total = count,
                    PageSize = pageSize,
                    currentPage = pageNumber,
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
}
