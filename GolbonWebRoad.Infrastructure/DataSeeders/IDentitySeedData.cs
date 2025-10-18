using GolbonWebRoad.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GolbonWebRoad.Infrastructure.DataSeeders
{
    public static class IdentitySeedData
    {
        public static async Task Initialize(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminRole = "Admin";
            string adminEmail = "golbon.taha@gmail.com";
            string adminPassword = "Golbon$021"; // این پسورد را در محیط واقعی امن کنید

            if (await roleManager.FindByNameAsync(adminRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true

                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }
        }
    }
}
