using Mango.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.Data
{
    public class AppDbContext: DbContext
    {
        //add sxolia
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {
                    
        }
        //add sxolia
        public DbSet<EmailLogger> EmailLoggers { get; set; }
    }
}
