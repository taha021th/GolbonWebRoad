using Microsoft.AspNetCore.Identity;

namespace GolbonWebRoad.Infrastructure.DataSeeders
{
    public static class IdentitySeedData
    {
        public static async Task Initialize(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminRole = "User";
            string adminEmail = "user@gmail.com";
            string adminPassword = "Password123!"; // این پسورد را در محیط واقعی امن کنید

            if (await roleManager.FindByNameAsync(adminRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new IdentityUser
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
