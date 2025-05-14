using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Repositoty
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
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
                    }
                );
            }
            _context.SaveChanges();
        }
    }
}
