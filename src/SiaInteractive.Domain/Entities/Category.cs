namespace SiaInteractive.Domain.Entities
{
    public partial class Category
    {
        public int CategoryID { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
