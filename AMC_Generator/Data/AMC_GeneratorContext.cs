using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AMC_Generator.Models;

namespace AMC_Generator.Data
{
    public class AMC_GeneratorContext : DbContext
    {
        public AMC_GeneratorContext (DbContextOptions<AMC_GeneratorContext> options)
            : base(options)
        {
        }

        public DbSet<AMC_Generator.Models.Owner> Owner { get; set; } = default!;
        public DbSet<AMC_Generator.Models.Building> Building { get; set; } = default!;
        public DbSet<AMC_Generator.Models.AMC> AMC { get; set; } = default!;
    }
}
