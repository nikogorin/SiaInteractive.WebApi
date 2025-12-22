namespace SiaInteractive.Domain.Entities
{
    public partial class Product
    {
        public int ProductID { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Image { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
