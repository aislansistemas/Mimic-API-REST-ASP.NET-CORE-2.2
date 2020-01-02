using AutoMapper;
using Mimicapi.v1.Models;
using Mimicapi.v1.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimicapi.Helpers
{
    public class DTOMapperProfile:Profile
    {
        public DTOMapperProfile()
        {
            CreateMap<Palavra, PalavraDTO>();
            CreateMap<PaginationList<Palavra>, PaginationList<PalavraDTO>>();
        }
    }
}
