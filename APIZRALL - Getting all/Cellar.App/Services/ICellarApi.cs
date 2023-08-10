using Apizr;
using Apizr.Logging.Attributes;
using Cellar.app.Models;
using Refit;

namespace Cellar.app.Services
{
    [WebApi("https://localhost:7015"), Log]
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
