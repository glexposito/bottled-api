using Bottled.Api.Infrastructure.Data.Mappings;
using Bottled.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottled.Api.Infrastructure.Data;

public class BottledContext : DbContext
{
    public BottledContext(DbContextOptions<BottledContext> options) : base(options) { }

    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new MessageConfiguration().Configure(modelBuilder.Entity<Message>());
    }
}
