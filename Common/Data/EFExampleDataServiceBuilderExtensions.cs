using System.Diagnostics.CodeAnalysis;
using EFExample.Common.Data.CodeFirst;
using EFExample.Common.Data.DBFirst;
using Microsoft.EntityFrameworkCore;

namespace EFExample.Common.Data;

[ExcludeFromCodeCoverage]
public static class EFExampleDataServiceBuilderExtensions
{
    public static void AddEFExampleDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DbFirstDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DBFirstDb")));
        services.AddDbContext<CodeFirstDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("CodeFirstDb")));
    }
}