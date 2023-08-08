using Apizr;
using Apizr.Logging.Attributes;
using Refit;
using Todo.App.Models;

namespace Todo.App.Services
{
    [WebApi("https://localhost:7015"), Log]
    public interface ITodoItemsApi
    {
        [Get("/todoitems")]
        Task<IEnumerable<TodoItem>> GetTodoItemsAsync();

        [Get("/todoitems/{id}")]
        Task<TodoItem> GetTodoItemDetailsAsync(int id);

        [Post("/todoitems")]
        Task<TodoItem> CreateTodoItemAsync(TodoItem item);
        
        [Put("/todoitems/{id}")]
        Task UpdateTodoItemAsync(int id, TodoItem item);
        
        [Delete("/todoitems/{id}")]
        Task DeleteTodoItemAsync(int id);
    }
}
