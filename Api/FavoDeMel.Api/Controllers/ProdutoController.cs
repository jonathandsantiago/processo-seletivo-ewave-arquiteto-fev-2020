using FavoDeMel.Api.Controllers.Common;
using FavoDeMel.Api.Dtos;
using FavoDeMel.Domain.Produtos;
using FavoDeMel.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FavoDeMel.Api.Controllers
{
    public class ProdutoController : ControllerBase<Produto, int, ProdutoDto, IProdutoService>
    {
        public ProdutoController(IProdutoService service) : base(service)
        { }

        /// <summary>
        /// Responsável por obter todos os produtos
        /// </summary>
        /// <returns>Lista das comandas em amberto</returns>
        [HttpGet("")]
        public virtual async Task<IActionResult> ObterTodos()
        {
            try
            {
                return Ok(await _appService.GetAll());
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
}
