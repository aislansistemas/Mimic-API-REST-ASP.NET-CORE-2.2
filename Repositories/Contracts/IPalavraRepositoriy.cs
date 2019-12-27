﻿using Mimicapi.Helpers;
using Mimicapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimicapi.Repositories.Contracts
{
    public interface IPalavraRepositoriy
    {
        PaginationList<Palavra> ObterPalavras(PalavraUrlQuery query);

        Palavra ObterId(int id);

        void Cadastrar(Palavra palavra);

        void Atualizar(Palavra palavra);

        void Deletar(int id);
    }
}
