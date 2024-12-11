using Microsoft.EntityFrameworkCore;

namespace HR_API.Data
{
    public class HR_APIContext : DbContext
    {
        public HR_APIContext(DbContextOptions<HR_APIContext> options)
            : base(options)
        {
        }

        public DbSet<HR_Platform.Models.Employee> Employee { get; set; } = default!;
    }
}
