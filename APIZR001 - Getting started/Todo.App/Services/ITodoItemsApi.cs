using Apizr;
using Refit;
using Todo.App.Models;

namespace Todo.App.Services
{
    [WebApi("https://localhost:7015")]
    public interface ITodoItemsApi
    {
        [Get("/todoitems")]
        Task<List<TodoItem>> GetTodoItemsAsync();
    }
}
