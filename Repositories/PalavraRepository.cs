using Microsoft.EntityFrameworkCore;
using Mimicapi.Contenxt;
using Mimicapi.Helpers;
using Mimicapi.Models;
using Mimicapi.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimicapi.Repositories
{
    public class PalavraRepository : IPalavraRepositoriy
    {
        private readonly MimicContext _banco;

        public PalavraRepository(MimicContext contexto)
        {
            _banco = contexto;
        }

        public void Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public void Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra=ObterId(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public Palavra ObterId(int id)
        {
            return _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public PaginationList<Palavra> ObterPalavras(PalavraUrlQuery query)
        {
            
            var item = _banco.Palavras.AsNoTracking().AsQueryable();
            var lista = new PaginationList<Palavra>();
            if (query.data.HasValue)
            {
                item = item.Where(a => a.Criado > query.data.Value || a.Atualizado > query.data.Value);
            }

            if (query.pagnumero.HasValue)
            {
                var quantidadeTotalRegistros = item.Count();
                item = item.Skip((query.pagnumero.Value - 1) * query.pagregistro.Value).Take(query.pagregistro.Value);

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.pagnumero.Value;
                paginacao.RegistroPorPagina = query.pagregistro.Value;
                paginacao.TotalRegistros = quantidadeTotalRegistros;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)quantidadeTotalRegistros / query.pagregistro.Value);

                lista.Paginacao = paginacao;
            }

            lista.AddRange(item.ToList());

            return lista;
        }
    }
}
