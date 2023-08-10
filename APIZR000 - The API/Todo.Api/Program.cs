using Microsoft.EntityFrameworkCore;
using Todo.Api;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoDb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();


var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
var version = $"{versionInfo.FileMajorPart}.{versionInfo.ProductMinorPart}.{versionInfo.ProductBuildPart}";
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc($"v1", new OpenApiInfo
    {
        Title = "Apizr - Todo demo",
        Contact = new OpenApiContact
        {
            Name = "Respawnsive",
            Email = "contact@respawnsive.com",
            Url = new Uri("https://respawnsive.com")
        },
        Description = "This is a demo api for the Apizr client library. Do not use this API for production. For more information please visit https://apizr.net",
        Version = version
    });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/uploads"
});

app.Map("/", async context =>
{
    await Task.CompletedTask;
    context.Response.Redirect("/swagger");
});

app.MapPost("/upload", async (IFormFile file, IHttpContextAccessor httpContextAccessor) =>
{
    var fileName = Path.GetFileName(file.FileName);
    var uniqueFileName = string.Concat(Path.GetFileNameWithoutExtension(fileName)
        , "_"
        , Guid.NewGuid().ToString().AsSpan(0, 4)
        , Path.GetExtension(fileName));

    var directoryPath = "Uploads";
    var filePath = Path.Combine(directoryPath, uniqueFileName);

    if (!Directory.Exists(directoryPath)) 
        Directory.CreateDirectory(directoryPath);

    await using var stream = File.OpenWrite(filePath);
    await file.CopyToAsync(stream);

    var scheme = httpContextAccessor.HttpContext!.Request.Scheme;
    var hostName = httpContextAccessor.HttpContext!.Request.Host.Value;
    var fileUri = $"{scheme}://{hostName}/{directoryPath}/{uniqueFileName}";

    return Results.Ok(fileUri);
});

var todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", GetAllTodos).WithOpenApi();
todoItems.MapGet("/complete", GetCompleteTodos).WithOpenApi();
todoItems.MapGet("/{id}", GetTodo).WithOpenApi();
todoItems.MapPost("/", CreateTodo).WithOpenApi();
todoItems.MapPut("/{id}", UpdateTodo).WithOpenApi();
todoItems.MapDelete("/{id}", DeleteTodo).WithOpenApi();

app.Run();

static async Task<IResult> GetAllTodos(TodoDbContext db)
{
    return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(TodoDbContext db)
{
    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDTO(x)).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDbContext db)
{
    return await db.Todos.FindAsync(id)
        is { } todo
            ? TypedResults.Ok(new TodoItemDTO(todo))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(TodoItemDTO todoItemDTO, TodoDbContext db)
{
    var todoItem = new Todo.Api.Todo
    {
        IsComplete = todoItemDTO.IsComplete,
        Name = todoItemDTO.Name
    };

    db.Todos.Add(todoItem);
    await db.SaveChangesAsync();

    todoItemDTO = new TodoItemDTO(todoItem);

    return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
}

static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDbContext db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = todoItemDTO.Name;
    todo.IsComplete = todoItemDTO.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDbContext db)
{
    if (await db.Todos.FindAsync(id) is { } todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();

        var todoItemDTO = new TodoItemDTO(todo);

        return TypedResults.Ok(todoItemDTO);
    }

    return TypedResults.NotFound();
}

static string CreateTempfilePath()
{
    var filename = $"{Guid.NewGuid()}.tmp";
    var directoryPath = Path.Combine("temp", "uploads");
    if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

    return Path.Combine(directoryPath, filename);
}