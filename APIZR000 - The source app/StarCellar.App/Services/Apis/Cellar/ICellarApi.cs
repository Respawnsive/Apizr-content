using Refit;
using StarCellar.App.Services.Apis.Cellar.Dtos;

namespace StarCellar.App.Services.Apis.Cellar
{
    public interface ICellarApi
    {
        [Get("/wines")]
        Task<IEnumerable<Wine>> GetWinesAsync();

        [Get("/wines/{id}")]
        Task<Wine> GetWineDetailsAsync(int id);

        [Post("/wines")]
        Task<Wine> CreateWineAsync(Wine item);
        
        [Put("/wines/{id}")]
        Task UpdateWineAsync(int id, Wine item);
        
        [Delete("/wines/{id}")]
        Task DeleteWineAsync(int id);
    }
}
