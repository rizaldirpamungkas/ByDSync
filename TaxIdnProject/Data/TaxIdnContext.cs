using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxIdnProject.Models;

namespace TaxIdnProject.Data
{
    public class TaxIdnContext : DbContext
    {
        public TaxIdnContext (DbContextOptions<TaxIdnContext> options)
            : base(options)
        {
        }

        public DbSet<TaxIdnProject.Models.EFakturContainer> EFakturContainer { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EFakturContainer>().ToTable("EFakturContainer");
        }
    }
}
