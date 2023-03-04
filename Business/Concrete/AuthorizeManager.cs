using Business.Interfaces;

namespace Business.Concrete
{
    public class AuthorizeManager : IAuthorizeService
    {
        public bool ValidateCredentials(string username, string password)
        {
            return username.Equals("admin") && password.Equals("1");
        }
    }
}
