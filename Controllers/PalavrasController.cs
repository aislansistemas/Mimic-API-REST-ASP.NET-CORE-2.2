using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mimicapi.Contenxt;
using Mimicapi.Helpers;
using Mimicapi.Models;
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
        private readonly MimicContext _banco;

        public PalavrasController(MimicContext mimic)
        {
            _banco = mimic;  
        }
        [HttpGet]
        [Route("")]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {   
            var item = _banco.Palavras.AsEnumerable();
            if (query.data.HasValue)
            {
                item = item.Where(a => a.Criado > query.data.Value || a.Atualizado> query.data.Value);
            }
            if (query.pagnumero.HasValue)
            {
                var quantidaderegistro = item.Count();
                item = item.Skip((query.pagnumero.Value-1)*query.pagregistro.Value).Take(query.pagregistro.Value);
                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.pagnumero.Value;
                paginacao.RegistroPorPagina = query.pagregistro.Value;
                paginacao.TotalRegistros=quantidaderegistro;
                paginacao.TotalPaginas=(int) Math.Ceiling((double)quantidaderegistro / query.pagregistro.Value);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));
                if (query.pagnumero > paginacao.TotalPaginas)
                {
                    return NotFound();
                }
            }
            return Ok(item);
        }
        [HttpGet]
        [Route("{id}")]
        public ActionResult Obter(int id)
        {
            var obj = _banco.Palavras.Find(id);
            if (obj == null)
                return NotFound();
            return Ok();
        }
        [HttpPost]
        [Route("")]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
            return Created($"api/palavras/{palavra.Id}",palavra);
        }
        [HttpPut]
        [Route("{id}")]
        public ActionResult Atualizar(int id,[FromBody]Palavra palavra)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
            if (obj == null)
                return NotFound();
            palavra.Id = id;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
            return Ok();
        }
        [HttpDelete]
        [Route("{id}")]
        public ActionResult Deletar(int id)
        {
            var palavra = _banco.Palavras.Find(id);
            if (palavra == null)
                return NotFound();
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
            return NoContent();
        }
        
    }
}
