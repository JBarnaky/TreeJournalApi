using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<Node> Nodes { get; set; }
    public DbSet<ExceptionLog> ExceptionLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Node>()
            .HasMany(n => n.Children)
            .WithOne()
            .HasForeignKey(n => n.ParentId);
    }
}
