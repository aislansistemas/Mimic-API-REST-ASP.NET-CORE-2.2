using Microsoft.EntityFrameworkCore;
using Mimicapi.v1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimicapi.Contenxt
{
    public class MimicContext:DbContext
    {
        public MimicContext(DbContextOptions<MimicContext>options):
            base(options)
        {

        }
        public DbSet<Palavra> Palavras { get; set; }
    }
}
