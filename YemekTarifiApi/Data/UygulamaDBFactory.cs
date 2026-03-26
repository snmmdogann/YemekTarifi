using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace YemekTarifiApi.Data
{
    public class UygulamaDBFactory : IDesignTimeDbContextFactory<TarifDbContext>
    {
        public TarifDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TarifDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=YemekTarifiDb;Trusted_Connection=True;");

            return new TarifDbContext(optionsBuilder.Options);
        }
    }
}