using AutoMapper;
using Mimicapi.Models;
using Mimicapi.Models.DTO;
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
        }
    }
}
