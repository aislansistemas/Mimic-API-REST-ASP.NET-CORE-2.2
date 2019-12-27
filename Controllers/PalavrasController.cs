using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mimicapi.Contenxt;
using Mimicapi.Helpers;
using Mimicapi.Models;
using Mimicapi.Repositories.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimicapi.Controllers
{   
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepositoriy _repository;

        public PalavrasController(IPalavraRepositoriy repo)
        {
            _repository = repo;  
        }
        [HttpGet]
        [Route("")]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _repository.ObterPalavras(query);

            if (query.pagnumero > item.Paginacao.TotalPaginas)
            {
                return NotFound();
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

            return Ok(item.ToList());
        }
        [HttpGet]
        [Route("{id}")]
        public ActionResult Obter(int id)
        {
            var obj = _repository.ObterId(id);
            if (obj == null)
                return NotFound();
            return Ok(obj);
        }
        [HttpPost]
        [Route("")]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _repository.Cadastrar(palavra);
            return Created($"api/palavras/{palavra.Id}",palavra);
        }
        [HttpPut]
        [Route("{id}")]
        public ActionResult Atualizar(int id,[FromBody]Palavra palavra)
        {
            var obj = _repository.ObterId(id);
            if (obj == null)
                return NotFound();
            palavra.Id = id;
            _repository.Atualizar(palavra);
            return Ok();
        }
        [HttpDelete]
        [Route("{id}")]
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
