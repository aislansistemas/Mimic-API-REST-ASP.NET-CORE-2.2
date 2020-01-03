using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mimicapi.Contenxt;
using Mimicapi.Helpers;
using Mimicapi.v1.Models;
using Mimicapi.v1.Models.DTO;
using Mimicapi.v1.Repositories.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimicapi.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.1")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepositoriy _repository;
        private readonly IMapper _mapper;
        public PalavrasController(IPalavraRepositoriy repo, IMapper mapper)
        {
            _repository = repo;
            _mapper = mapper;
        }

        /// <summary>
        /// Operação lista todas as palavras existentes no banco de dados.
        /// </summary>
        /// <param name="query">Filtros de pesquisa</param>
        /// <returns>Listagem de palavrasa</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpGet("",Name ="ObterTodas")]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _repository.ObterPalavras(query);
            var lista=_mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDTO>>(item);

            if (item.Results.Count==0)
                return NotFound();
            
            foreach (var palavra in lista.Results)
            {
                palavra.Links.Add(new LinkDTO("self", Url.Link("Obter", new { id = palavra.Id }), "GET")
                );
            }
            lista.Links.Add(new LinkDTO("self", Url.Link("ObterTodas", query), "GET")
                );
            if (item.Paginacao != null)
            {
                if (query.pagnumero + 1 <= item.Paginacao.TotalPaginas) {
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));
                    var querystring = new PalavraUrlQuery() { pagnumero = query.pagnumero + 1, pagregistro = query.pagregistro, data = query.data };
                    lista.Links.Add(new LinkDTO("next", Url.Link("ObterTodas",querystring), "GET")
                    );
                }
                if (query.pagnumero - 1 > 0)
                {
                    var querystring = new PalavraUrlQuery() { pagnumero = query.pagnumero - 1, pagregistro = query.pagregistro, data = query.data };
                    lista.Links.Add(new LinkDTO("prev", Url.Link("ObterTodas",querystring), "GET")
                    );
                }
            }
            return Ok(lista);
        }

        /// <summary>
        /// Operação que pega uma única palavra do banco de dados.
        /// </summary>
        /// <param name="id">Código identificador da palavra</param>
        /// <returns>Um objeto de palavra</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpGet("{id}", Name = "Obter")]
        public ActionResult Obter(int id)
        {
            var obj = _repository.ObterId(id);
            if (obj == null)
                return NotFound();
            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(obj);
            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("Obter", new { id = palavraDTO.Id }), "GET")
                );
            palavraDTO.Links.Add(
                new LinkDTO("update", Url.Link("Atualizar", new { id = palavraDTO.Id }), "PUT")
                );
            palavraDTO.Links.Add(
                new LinkDTO("delete", Url.Link("Deletar", new { id = palavraDTO.Id }), "DELETE")
                );

            return Ok(palavraDTO);
        }

        /// <summary>
        /// Operação que realiza o cadastro da palavra
        /// </summary>
        /// <param name="palavra">Objeto referente a Entidade Palavra</param>
        /// <returns>Um objeto palavra com seu respectivo id</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpPost]
        [Route("")]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            if (palavra == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            palavra.Ativo = true;
            palavra.Criado = DateTime.Now;
            _repository.Cadastrar(palavra);
            var palavradto= _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavradto.Links.Add(
                new LinkDTO("self", Url.Link("Obter", new { id = palavradto.Id }), "GET")
                );
            return Created($"api/palavras/{palavra.Id}", palavra);
        }

        /// <summary>
        /// Operação que realiza a substituição de dados de uma palavra específica
        /// </summary>
        /// <param name="id">Código identificador da palavra a ser alterada</param>
        /// <param name="palavra">Objeto palavra com dados para alteração</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpPut("{id}",Name ="Atualizar")]
        public ActionResult Atualizar(int id,[FromBody]Palavra palavra)
        {
            var obj = _repository.ObterId(id);
            if (obj == null)
                return NotFound();
            if (palavra == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            palavra.Id = id;
            palavra.Ativo = obj.Ativo;
            palavra.Criado = obj.Criado;
            palavra.Atualizado = DateTime.Now;
            _repository.Atualizar(palavra);
            var palavradto = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavradto.Links.Add(
                new LinkDTO("self", Url.Link("Obter", new { id = palavradto.Id }), "GET")
                );
            return Ok();
        }

        /// <summary>
        /// Operação que desativa uma palavra do sistema
        /// </summary>
        /// <param name="id">Código identificador da palavra</param>
        /// <returns></returns>
        [MapToApiVersion("1.1")]
        [HttpDelete("{id}",Name ="Deletar")]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.ObterId(id);
            if (palavra == null)
                return NotFound();
            _repository.Deletar(id);
            return NoContent();
        }
        
    }
}
