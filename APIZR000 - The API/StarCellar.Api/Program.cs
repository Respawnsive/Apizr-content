using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using StarCellar.Api.Data;
using StarCellar.Api.Handlers;
using StarCellar.Api.Utils;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using static Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var accessTokenSecret = builder.Configuration["Jwt:AccessTokenSecret"];
var isProduction = builder.Environment.IsProduction();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.Converters.Add(new UserConverter());
});

builder.Services.AddEndpointsApiExplorer();

var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
var version = $"{versionInfo.FileMajorPart}.{versionInfo.ProductMinorPart}.{versionInfo.ProductBuildPart}";
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc($"v1", new OpenApiInfo
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

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        }
    );
});

builder.Services.AddDbContext<AppDbContext>(
    optionsBuilder =>
        optionsBuilder
            .UseSqlite("Data Source=app.db")
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            .EnableDetailedErrors()
            .ConfigureWarnings(b => b.Log(ConnectionOpened, CommandExecuted, ConnectionClosed))
);

builder.Services.AddDbContext<TokenDbContext>(
    optionsBuilder => optionsBuilder.UseInMemoryDatabase("Tokens")
);

builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddSingleton<TokenValidator>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = isProduction;
        options.Password.RequireLowercase = isProduction;
        options.Password.RequireNonAlphanumeric = isProduction;
        options.Password.RequireUppercase = isProduction;
        if (isProduction)
        {
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 3;
        }
    })
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecret)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.SaveToken = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "admin",
        policy => policy.RequireAuthenticatedUser().RequireClaim("role", "admin")
    );
    options.AddPolicy(
        "user",
        policy => policy.RequireAuthenticatedUser().RequireClaim("role", "user")
    );
    options.AddPolicy(
        "own-profile",
        policy =>
            policy
                .RequireAuthenticatedUser()
                .RequireAssertion(context =>
                {
                    string userIdFromPath = "";
                    if (context.Resource is HttpContext http)
                        userIdFromPath = http.Request.Path.Value.Split('/').Last();
                    else
                        return false;
                    UserClaimsValidator.TryValidate(context.User, out var user, out var errMsg);
                    var userIdFromClaims = user.Id.ToString();
                    return userIdFromPath == userIdFromClaims;
                })
    );
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    if (Directory.Exists(Constants.DirectoryPath))
        Directory.Delete(Constants.DirectoryPath, true);

    Directory.CreateDirectory(Constants.DirectoryPath);
});

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("signup", Users.SignUpAsync);
app.MapPost("signin", Users.SignInAsync);
app.MapPost("refresh", Users.RefreshTokenAsync);
app.MapDelete("signout", Users.SignOutAsync);
app.MapGet("user/{id}", Users.GetProfileAsync).RequireAuthorization("own-profile");

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

app.MapPost("/upload", Files.UploadAsync);

var wineRoutes = app.MapGroup("/wines");
wineRoutes.MapGet("/", Wines.GetAllWines).WithOpenApi();
wineRoutes.MapGet("/{id}", Wines.GetWine).WithOpenApi();
wineRoutes.MapPost("/", Wines.CreateWine).WithOpenApi();
wineRoutes.MapPut("/{id}", Wines.UpdateWine).WithOpenApi();
wineRoutes.MapDelete("/{id}", Wines.DeleteWine).WithOpenApi();

app.Run();