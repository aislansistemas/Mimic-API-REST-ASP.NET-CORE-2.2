using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mimicapi.v1.Models
{
    public class Palavra
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="{0} obrigatório")]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "{0} obrigatório")]
        public int Pontuacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime Criado { get; set; }
        public DateTime? Atualizado { get; set; }
    }
}
