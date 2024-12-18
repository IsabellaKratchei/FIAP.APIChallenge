using FIAP.APIRegiao.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class RegiaoDbContext : DbContext
{
    public RegiaoDbContext(DbContextOptions<RegiaoDbContext> options) : base(options) { }

    public DbSet<RegiaoModel> DDDs { get; set; }
}
