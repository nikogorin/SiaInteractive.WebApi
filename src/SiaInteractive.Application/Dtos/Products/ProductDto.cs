using SiaInteractive.Application.Dtos.Categories;

namespace SiaInteractive.Application.Dtos.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public List<CategoryDto>? Categories { get; set; }
    }
}
