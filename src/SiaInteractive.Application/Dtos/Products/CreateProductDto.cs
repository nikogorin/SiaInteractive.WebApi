namespace SiaInteractive.Application.Dtos.Products
{
    public class CreateProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public List<int>? CategoryIds { get; set; }
    }
}