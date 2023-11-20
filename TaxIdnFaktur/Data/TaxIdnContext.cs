using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxIdnFaktur.Models;

namespace TaxIdnFaktur.Data
{
    public class TaxIdnContext : DbContext
    {
        public TaxIdnContext (DbContextOptions<TaxIdnContext> options)
            : base(options)
        {
        }

        public DbSet<TaxIdnFaktur.Models.TaxFakturContainer> TaxFakturContainer { get; set; } = default!;
    }
}
