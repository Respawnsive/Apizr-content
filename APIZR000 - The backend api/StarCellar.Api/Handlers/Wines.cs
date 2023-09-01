using Microsoft.EntityFrameworkCore;
using MiniValidation;
using StarCellar.Api.Data;

namespace StarCellar.Api.Handlers
{
    internal static class Wines
    {
        internal static async Task<IResult> GetAllWines(AppDbContext appContext) => TypedResults.Ok(await appContext
            .Wines.Select(wine => new WineDTO(wine.Id, wine.Name, wine.Description, wine.ImageUrl, wine.Stock,
                wine.Score, wine.OwnerId)).ToArrayAsync());

        internal static async Task<IResult> GetWine(Guid id, AppDbContext appContext)
        {
            return await appContext.Wines.FindAsync(id)
                is { } wine
                ? TypedResults.Ok(new WineDTO(wine.Id, wine.Name, wine.Description, wine.ImageUrl, wine.Stock,
                    wine.Score, wine.OwnerId))
                : TypedResults.NotFound();
        }

        internal static async Task<IResult> CreateWine(WineDTO wineDto, AppDbContext appContext)
        {
            if (wineDto is null) 
                return TypedResults.BadRequest("Must include a Todo.");

            if (!MiniValidator.TryValidate(wineDto, out var errors)) 
                return TypedResults.BadRequest(errors);

            var (id, name, description, imageUrl, stock, score, ownerId) = wineDto;

            if (ownerId == Guid.Empty) 
                return TypedResults.BadRequest("Invalid `ownerId`.");

            if (id == Guid.Empty) 
                id = Guid.NewGuid();

            var owner = await appContext.Users.FindAsync(ownerId);
            if (owner is null) 
                return TypedResults.NotFound($"User with Id {ownerId} not found.");

            var wineFromDb = await appContext.Wines.FindAsync(id);
            if (wineFromDb is not null) 
                return TypedResults.Conflict("A wine with this ID already exists.");

            var wine = new Wine(id, name, description, imageUrl, stock, score, ownerId, owner);

            appContext.Wines.Add(wine);
            await appContext.SaveChangesAsync();

            wineDto = new WineDTO(wine.Id, wine.Name, wine.Description, wine.ImageUrl, wine.Stock,
                wine.Score, wine.OwnerId);

            return TypedResults.Created($"/wines/{wineDto.Id}", wineDto);
        }

        internal static async Task<IResult> UpdateWine(Guid id, WineDTO wineDto, AppDbContext appContext)
        {
            if (wineDto is null)
                return TypedResults.BadRequest("Must include a Todo.");

            if (!MiniValidator.TryValidate(wineDto, out var errors))
                return TypedResults.BadRequest(errors);

            var wine = await appContext.Wines.FindAsync(id);
            if (wine is null) 
                return TypedResults.NotFound();

            wine.Name = wineDto.Name;
            wine.Description = wineDto.Description;
            wine.ImageUrl = wineDto.ImageUrl;
            wine.Stock = wineDto.Stock;
            wine.Score = wineDto.Score;

            await appContext.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        internal static async Task<IResult> DeleteWine(Guid id, AppDbContext appContext)
        {
            if (await appContext.Wines.FindAsync(id) is not { } wine) 
                return TypedResults.NotFound();

            appContext.Wines.Remove(wine);
            await appContext.SaveChangesAsync();

            return TypedResults.NoContent();

        }
    }
}
