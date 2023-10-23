using Microsoft.EntityFrameworkCore;

namespace CDRModel
{
    public class CDRContext : DbContext
    {
        public CDRContext(DbContextOptions<CDRContext> options) : base(options)
        {
        }

        public DbSet<Call>? Calls { get; private set; }
    }
}
