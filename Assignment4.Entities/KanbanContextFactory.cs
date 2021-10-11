using Assignment4.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Assignment4
{
    public class KanbanContextFactory : IDesignTimeDbContextFactory<KanbanContext>
    {
        public KanbanContext CreateDbContext(string[] args)
        {

            var connectionString = "Server=localhost;Database=Futurama;User Id=sa;Password=6d874d0d-33db-43fe-8af7-752c5562db3a";

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>()
                .UseSqlServer(connectionString);

            return new KanbanContext(optionsBuilder.Options);
        }

        public static void Seed(KanbanContext context)
        {
            
        }

        //method for populating database
        /*public static void Seed(KanbanContext context)
        {
            context.Database.ExecuteSqlRaw("DELETE dbo.Tasks");
            context.Database.ExecuteSqlRaw("DELETE dbo.Users");
            context.Database.ExecuteSqlRaw("DELETE dbo.Task");
            //context.Database.ExecuteSqlRaw("DELETE dbo.TaskTag");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tasks', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Users', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Task', RESEED, 0)");

            context.SaveChanges();
        }
        */
    }
}