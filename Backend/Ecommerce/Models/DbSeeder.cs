using Microsoft.AspNetCore.Identity;
using Ecommerce.Models;

namespace Ecommerce.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDefaultDataAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // SECURITY FIX: Grab the configuration tools
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            // 1. SEED ROLES
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // 2. SEED ADMIN USER
            var adminEmail = "admin@foodstore.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FirstName = "Master",
                    LastName = "Chef",
                    EmailConfirmed = true
                };

                // SECURITY FIX: The code looks for a secret named "AdminPassword" in Azure. 
                // If it doesn't find it (like on your local laptop), it uses a safe dummy password.
                var securePassword = config["AdminPassword"] ?? "LocalTestPassword123!";
                var createAdminResult = await userManager.CreateAsync(newAdmin, securePassword);

                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

            // 3. SEED FOOD CATEGORIES AND PRODUCTS
            if (!context.Categories.Any())
            {
                // Create Categories
                var bakeryCategory = new Category { Name = "Bakery & Pastries", ImageUrl = "https://bakerpedia.com/wp-content/uploads/2020/06/Pastry_baking-processes-e1593464950587.jpg" };
                var produceCategory = new Category { Name = "Fresh Produce", ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS4P6aH2D6FWRrhCGVB4LjOmvIotIxs5sf6Bg&s" };
                var beverageCategory = new Category { Name = "Beverages", ImageUrl = "https://www.cargill.com/image/1432079656479/preview-beverages-390x225.jpg?v=1483016365000" };

                context.Categories.AddRange(bakeryCategory, produceCategory, beverageCategory);
                await context.SaveChangesAsync();

                // Create Products
                if (!context.Products.Any())
                {
                    var defaultProducts = new List<Product>
                    {
                        new Product
                        {
                            ProductName = "Artisan Sourdough Loaf",
                            CategoryId = bakeryCategory.Id,
                            Price = 7.99m,
                            Stock = 20,
                            ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT1_FHCO8-v1jOkGOVyK71lPcexLaCk1eQnKQ&s",
                            IsArchived = false
                        },
                        new Product
                        {
                            ProductName = "Butter Croissant (4 Pack)",
                            CategoryId = bakeryCategory.Id,
                            Price = 12.50m,
                            Stock = 15,
                            ImageUrl = "https://la-marg.com/cdn/shop/products/APileofButterCroissantsLBM-80_1_900x.jpg?v=1636598269",
                            IsArchived = false
                        },
                        new Product
                        {
                            ProductName = "Organic Hass Avocados",
                            CategoryId = produceCategory.Id,
                            Price = 5.99m,
                            Stock = 50,
                            ImageUrl = "https://images.squarespace-cdn.com/content/v1/52e42689e4b015483fbf2e93/1426813727960-J0I79SNTMYNNKW1FIYN7/IMG_6195.jpg?format=1000w",
                            IsArchived = false
                        },
                        new Product
                        {
                            ProductName = "Cold Brew Coffee Concentrate",
                            CategoryId = beverageCategory.Id,
                            Price = 14.00m,
                            Stock = 30,
                            ImageUrl = "https://www.gradyscoldbrew.com/cdn/shop/articles/2019-07-02_Gradys_Rooftop_mark-weinberg_0159_2_1000x1000.jpg?v=1697813101",
                            IsArchived = false
                        }
                    };

                    context.Products.AddRange(defaultProducts);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}