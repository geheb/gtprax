namespace GtPrax.Application.Identity;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

public interface IIdentityService
{
    Task<IdentityResult> SignIn(string email, string password);
    Task SignOutCurrentUser();
}
