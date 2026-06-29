using AutoMapper;
using KIM.DAL.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Tests.Infrastructure;

public abstract class TestFixtureBase
{
    protected static (KimDbContext DbContext, SqliteConnection Connection) CreateDbContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<KimDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new KimDbContext(options);
        dbContext.Database.EnsureCreated();

        return (dbContext, connection);
    }

    protected static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(KIM.BL.Extensions.ServiceCollectionExtensions).Assembly);
        });

        return configuration.CreateMapper();
    }
}