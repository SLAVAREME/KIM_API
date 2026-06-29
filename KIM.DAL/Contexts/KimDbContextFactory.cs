using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KIM.DAL.Contexts;

public class KimDbContextFactory : IDesignTimeDbContextFactory<KimDbContext>
{
    private const string DefaultConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=kim;Integrated Security=True;Encrypt=False;";

    public KimDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<KimDbContext>();
        optionsBuilder.UseSqlServer(DefaultConnectionString);

        return new KimDbContext(optionsBuilder.Options);
    }
}