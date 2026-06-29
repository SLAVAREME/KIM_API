using KIM.DAL.Contexts;
using KIM.DAL.RepositoryBase;
using KIM.DAL.RepositoryBase.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KIM.DAL.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ConnectionStringName = "Kim";

    public static IServiceCollection AddKimDal(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=kim;Integrated Security=True;Encrypt=False;";

        services.AddDbContext<KimDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));

        return services;
    }
}