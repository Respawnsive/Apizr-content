using Apizr;
using Apizr.Configuring.Request;
using Apizr.Logging.Attributes;
using Refit;
using StarCellar.App.Services.Apis.Cellar.Dtos;
using StarCellar.App.Services.Apis.User.Dtos;

namespace StarCellar.App.Services.Apis.Cellar
{
    [WebApi, Log]
    public interface ICellarApi
    {
        [Get("/wines")]
        Task<IEnumerable<WineDTO>> GetWinesAsync([RequestOptions] IApizrRequestOptions options);

        [Get("/wines/{id}")]
        Task<WineDTO> GetWineDetailsAsync(int id, [RequestOptions] IApizrRequestOptions options);

        [Post("/wines")]
        Task<WineDTO> CreateWineAsync(WineDTO item, [RequestOptions] IApizrRequestOptions options);
        
        [Put("/wines/{id}")]
        Task UpdateWineAsync(int id, WineDTO item, [RequestOptions] IApizrRequestOptions options);
        
        [Delete("/wines/{id}")]
        Task DeleteWineAsync(int id, [RequestOptions] IApizrRequestOptions options);
    }
}
