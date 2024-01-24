using Microsoft.AspNetCore.Identity;

namespace Boiler.Server;

public static class Seed
{
    public static async Task SeedUp(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Check if the admin role exists and create if not
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        if (!await roleManager.RoleExistsAsync("Technician"))
        {
            await roleManager.CreateAsync(new IdentityRole("Technician"));
        }

        if (!await roleManager.RoleExistsAsync("Initializer"))
        {
            await roleManager.CreateAsync(new IdentityRole("Initializer"));
        }

        var adminUser = await userManager.FindByEmailAsync("admin@training.com");
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin@training.com",
                Email = "admin@training.com",
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(adminUser, "Super@1234");

            // Add the admin user to the admin role
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}