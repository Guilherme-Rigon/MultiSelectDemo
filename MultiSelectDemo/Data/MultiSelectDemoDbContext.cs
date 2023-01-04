using Microsoft.EntityFrameworkCore;
using MultiSelectDemo.Models;

namespace MultiSelectDemo.Data
{
    public class MultiSelectDemoDbContext : DbContext
    {
        public MultiSelectDemoDbContext(DbContextOptions<MultiSelectDemoDbContext> options) : base(options)
        {
        }

        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}
