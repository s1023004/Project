using Microsoft.EntityFrameworkCore;

namespace Project.Models
{
    //應對後端資料庫
    public class NorthwindDB:DbContext
    {
        public NorthwindDB(DbContextOptions options) : base(options) 
        {
            Console.WriteLine("DbContext物件起來了");
        }
        public DbSet<Customers> Customers { set; get; }
    }
}
