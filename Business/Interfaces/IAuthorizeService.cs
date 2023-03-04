namespace Business.Interfaces
{
    public interface IAuthorizeService
    {
        bool ValidateCredentials(string username, string password);
    }
}
