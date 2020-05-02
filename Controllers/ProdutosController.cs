using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CirrasTec.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CirrasTec.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository Repositorio;

        public ProdutosController(IProdutoRepository repositorio)
        {
            Repositorio = repositorio;
        }

        [HttpPost]
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Criar([FromBody]Produto produto)
        {
            if (produto.Codigo == "") return BadRequest("Código do Produto não informado");
            if (String.IsNullOrEmpty(produto.Descricao)) return BadRequest("Descrição do Produto não informado");

            Repositorio.Inserir(produto);
            return Created(nameof(Criar), produto);
        }

        [HttpGet]
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        [ResponseCache(Duration = 120)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Produces("application/json","application/xml")]
        public IActionResult Obter()
        {
            var lista = Repositorio.Obter();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        [ApiVersion("1.0", Deprecated = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult Obter(int id)
        {
            var produto = Repositorio.Obter(id);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        [HttpGet("{codigo}")]
        [ApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult Obter(string codigo)
        {
            var produto = Repositorio.ObterPorCodigo(codigo);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        [HttpGet("")]
        [ApiVersion("3.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult ObterTodos()
        {
            List<String> lista = new List<string>();
            for (int i = 1; i<= 1000; i++)
            {
                lista.Add("Indice: " + i.ToString() );
            }
            
            return Ok(String.Join(" / ",lista));
        }

        [HttpPut]
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult Atualizar([FromBody]Produto produto)
        {
            var prd = Repositorio.Obter(produto.Id);
            if (prd == null)
            {
                return NotFound();
            }

            if (produto.Codigo == "") return BadRequest("Código do Produto não informado");
            if (String.IsNullOrEmpty(produto.Descricao)) return BadRequest("Descrição do Produto não informado");

            Repositorio.Editar(produto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult Apagar(int id)
        {
            var prd = Repositorio.Obter(id);
            if (prd == null)
            {
                return NotFound();
            }
            Repositorio.Excluir(prd);
            return Ok();
        }
    }
}