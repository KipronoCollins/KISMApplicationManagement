using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KISMApplicationManagement.Data
{
    public class ApplicationDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(
            string[] args)
        {
            var connectionString =
                Environment.GetEnvironmentVariable(
                    "KISM_MYSQL_CONNECTION");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "KISM_MYSQL_CONNECTION environment variable " +
                    "is not configured.");
            }

            var optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseMySQL(connectionString);

            return new ApplicationDbContext(
                optionsBuilder.Options);
        }
    }
}