using Microsoft.AspNetCore.Mvc;
using Mimicapi.Helpers;
using Mimicapi.v1.Models.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimicapi.v2.Controllers
{
    namespace MimicAPI.V2.Controllers
    {
        [ApiController]
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiVersion("2.0")]
        public class PalavrasController : ControllerBase
        {
            /// <summary>
            /// Operação lista todas as palavras existentes no banco de dados.
            /// </summary>
            /// <param></param>
            /// <returns>Listagem de palavrasa</returns>
            [HttpGet("", Name = "ObterTodas")]
            public string ObterTodas()
            {
                return "Versão 2.0";
            }
        }
    }
}
