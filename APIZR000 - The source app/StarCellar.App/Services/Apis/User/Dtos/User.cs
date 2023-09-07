namespace StarCellar.App.Services.Apis.User.Dtos
{
    public record User(Guid Id, string UserName, string FullName, string Email, int Age, string Role, string Address);
}
