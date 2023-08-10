using Apizr;
using Apizr.Logging.Attributes;
using Refit;
using StarCellar.App.Models;

namespace StarCellar.App.Services
{
    [WebApi, Log]
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
