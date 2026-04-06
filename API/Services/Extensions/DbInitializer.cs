using BookingSystem.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Api.Data;

public static class DbInitializer
{
    public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();


            string[] roleNames = { "Admin", "User", "Moder" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }

            var adminEmail = "admin@booking.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin =
                new AppUser(
           adminEmail,
           "System Administrator",
           "System",
           "Admin",
           "Admin",
           "+79292376296");


                var result = await userManager.CreateAsync(newAdmin, "Admin@12345");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}
