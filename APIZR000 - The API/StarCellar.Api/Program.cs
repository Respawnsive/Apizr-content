using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using StarCellar.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CellarDbContext>(opt => opt.UseInMemoryDatabase("WineDb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();


var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
var version = $"{versionInfo.FileMajorPart}.{versionInfo.ProductMinorPart}.{versionInfo.ProductBuildPart}";
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc($"v1", new OpenApiInfo
    {
        Title = "Apizr - StarCellar demo",
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

var wineRoutes = app.MapGroup("/wines");

wineRoutes.MapGet("/", GetAllWines).WithOpenApi();
wineRoutes.MapGet("/{id}", GetWine).WithOpenApi();
wineRoutes.MapPost("/", CreateWine).WithOpenApi();
wineRoutes.MapPut("/{id}", UpdateWine).WithOpenApi();
wineRoutes.MapDelete("/{id}", DeleteWine).WithOpenApi();

app.Run();

static async Task<IResult> GetAllWines(CellarDbContext db)
{
    return TypedResults.Ok(await db.Wines.Select(x => new WineDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetWine(int id, CellarDbContext db)
{
    return await db.Wines.FindAsync(id)
        is { } wine
            ? TypedResults.Ok(new WineDTO(wine))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateWine(WineDTO wineDTO, CellarDbContext db)
{
    var wine = new Wine
    {
        Stock = wineDTO.Stock,
        Name = wineDTO.Name
    };

    db.Wines.Add(wine);
    await db.SaveChangesAsync();

    wineDTO = new WineDTO(wine);

    return TypedResults.Created($"/wines/{wine.Id}", wineDTO);
}

static async Task<IResult> UpdateWine(int id, WineDTO wineDTO, CellarDbContext db)
{
    var wine = await db.Wines.FindAsync(id);

    if (wine is null) return TypedResults.NotFound();

    wine.Name = wineDTO.Name;
    wine.Stock = wineDTO.Stock;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteWine(int id, CellarDbContext db)
{
    if (await db.Wines.FindAsync(id) is { } wine)
    {
        db.Wines.Remove(wine);
        await db.SaveChangesAsync();

        var wineDTO = new WineDTO(wine);

        return TypedResults.Ok(wineDTO);
    }

    return TypedResults.NotFound();
}