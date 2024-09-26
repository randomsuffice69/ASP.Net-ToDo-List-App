using Microsoft.EntityFrameworkCore;

namespace ToDoList.Models
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        public DbSet<ToDo> ToDos { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Status> Statuses { get; set; } = null!;

        //seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category {  CategoryID = "work", Name = "Work"},
                new Category {  CategoryID = "home", Name = "Home"},
                new Category {  CategoryID = "driving", Name = "Driving"},
                new Category {  CategoryID = "shop", Name = "Shop"},
                new Category {  CategoryID = "school", Name = "School"}
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { StatusID = "open", Name = "Open"},
                new Status { StatusID = "closed", Name = "Completed"}
             );
        }
    }
}
