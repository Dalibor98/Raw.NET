using Northwind.Services.Repositories;

namespace Northwind.Services.EntityFramework.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public virtual ICollection<Product> Products { get; } = new List<Product>();
    }
}
