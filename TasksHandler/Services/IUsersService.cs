using System.Security.Claims;

namespace TasksHandler.Services
{
    public interface IUsersService
    {
        string getUserId();
    }

    public class UsersService: IUsersService
    {
        private HttpContext httpContext;

        public UsersService(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }
        public string getUserId()
        {
            if(httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims
                    .Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                return idClaim.Value;
            }
            else
            {
                throw new Exception("User is not authenticaded");
            }
        }
    }
}
