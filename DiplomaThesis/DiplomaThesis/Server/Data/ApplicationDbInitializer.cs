using DiplomaThesis.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace DiplomaThesis.Server.Data;

public class ApplicationDbInitializer
{
    public static void SeedUsers(UserManager<IdentityUser> userManager)
    {
        if (userManager.FindByEmailAsync("admin@admin.cz").Result==null)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = "admin@admin.cz",
                Email = "admin@admin.cz"
            };

            IdentityResult result = userManager.CreateAsync(user, "admin").Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }
    }
}