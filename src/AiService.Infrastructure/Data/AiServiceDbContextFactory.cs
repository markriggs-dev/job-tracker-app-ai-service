using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AiService.Infrastructure.Data;

public class AiServiceDbContextFactory : IDesignTimeDbContextFactory<AiServiceDbContext>
{
    public AiServiceDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AiServiceDbContext>()
            .UseNpgsql("Host=localhost;Database=jobtracker;Username=jobtracker;Password=jobtracker")
            .Options;
        return new AiServiceDbContext(options);
    }
}
