using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace backend.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().Load();

        var keyFile = Environment.GetEnvironmentVariable("KEYFILE");
        string connectionString;

        if (!string.IsNullOrWhiteSpace(keyFile))
        {
            KeyFileReader.Read(keyFile);
            connectionString = KeyFileReader.ConnectionString;
        }
        else
        {
            connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? throw new InvalidOperationException("Either KEYFILE or DATABASE_URL must be set in .env");
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
