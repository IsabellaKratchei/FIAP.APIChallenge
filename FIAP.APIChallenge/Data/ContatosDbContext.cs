using FIAP.APIContato.Models;
using Microsoft.EntityFrameworkCore;

namespace FIAP.APIContato.Data
{
    public class ContatosDbContext : DbContext
    {
        public ContatosDbContext(DbContextOptions<ContatosDbContext> options) : base(options) { }

        public DbSet<ContatoModel> Contatos { get; set; }
    }
}
