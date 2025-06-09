using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Repository
{
    public class SeedData
    {
        public static async Task SeedingData(DataContext _context, IServiceProvider serviceProvider)
        {
            _context.Database.Migrate();

            // Seed roles
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var rolesToSeed = new List<IdentityRole>
                {
                    new IdentityRole
                    {
                        Id = "1",
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
                    new IdentityRole
                    {
                        Id = "2",
                        Name = "Customer",
                        NormalizedName = "CUSTOMER"
                    },
                    new IdentityRole
                    {
                        Id = "3",
                        Name = "Author",
                        NormalizedName = "AUTHOR"
                    }
                };

            foreach (var role in rolesToSeed)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(role);
                }
            }
            if (!_context.Products.Any())
            {
                CategoryModel Macbook = new CategoryModel
                {
                    Name = "Macbook",
                    Description = "Expensive Product",
                    Slug = "macbook",
                    Status = "Active"
                };
                CategoryModel PC = new CategoryModel
                {
                    Name = "PC",
                    Description = "Large Product",
                    Slug = "pc",
                    Status = "Active"
                };
                BrandModel Apple = new BrandModel
                {
                    Name = "Apple",
                    Description = "Largest Brand",
                    Slug = "apple",
                    Status = "Active"
                };
                BrandModel Samsung = new BrandModel
                {
                    Name = "Samsung",
                    Description = "Large Brand",
                    Slug = "samsung",
                    Status = "Active"
                };
                _context.Products.AddRange(
                    new ProductModel
                    {
                        Name = "Macbook",
                        Slug = "macbook",
                        Description = "Macbook is the best",
                        Price = 2000,
                        Image = "mac.jpg",
                        Category = Macbook,
                        Brand = Apple,
                        StockQuantity = 98,
                    },
                    new ProductModel
                    {
                        Name = "PC",
                        Slug = "pc",
                        Description = "PC is the best",
                        Price = 1000,
                        Image = "pc.jpg",
                        Category = PC,
                        Brand = Samsung,
                        StockQuantity = 92,
                    }
                );
                _context.SaveChanges();
            }
        }
    }
}
