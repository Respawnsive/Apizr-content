using Microsoft.EntityFrameworkCore;

namespace Todo.Api
{
    internal class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options) { }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}
