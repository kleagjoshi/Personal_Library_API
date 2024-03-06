using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalLibraryApi.Data.Models;
using PersonalLibraryApi.Data.ViewModels.Authentication;

namespace PersonalLibraryApi.Data
{
    public class AppDbInitializer
    {

        public static async Task SeedRoles(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //check if this role exists in db - if not add it to db
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));


                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                // Now that roles are created, let's add the admin user
                await SeedAdminUser(serviceScope.ServiceProvider);
            }
        }

        private static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Check if the admin user already exists
            var adminUser = await userManager.FindByEmailAsync("admin@admin.com");

            if (adminUser == null)
            {
                // Create the admin user
                adminUser = new ApplicationUser
                {
                    UserName = "AdminAdmin",
                    Email = "admin@admin.com",
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    // Assign the 'Admin' role to the admin user
                    await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
                }
            }
        }



    }
}




    

