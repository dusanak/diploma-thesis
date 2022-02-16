using DiplomaThesis.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace DiplomaThesis.Server.Data;

public class ApplicationDbInitializer
{
    public static void SeedUsers(UserManager<ApplicationUser> userManager)
    {
        if (userManager.FindByEmailAsync("admin@admin.cz").Result==null)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = "admin@admin.cz",
                Email = "admin@admin.cz"
            };

            IdentityResult result = userManager.CreateAsync(user, "admin1234").Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }
    }
}