using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Data
{
    public class TodoListDbContext : IdentityDbContext<IdentityUser>
    {
        public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("TaskItems");

                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title).IsRequired().HasMaxLength(200); 
                entity.Property(t => t.Description).HasMaxLength(1000);

                entity.Property(t => t.IsCompleted).IsRequired(); 

                entity.Property(t => t.UserId).IsRequired();

                entity.HasOne<IdentityUser>()
                      .WithMany()
                      .HasForeignKey(t => t.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}