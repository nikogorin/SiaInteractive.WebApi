namespace SiaInteractive.Application.Interfaces
{
    public interface ICategoryValidatorService
    {
        Task<bool> AllCategoriesExist(List<int> categoryIds, CancellationToken cancellationToken);
        bool NotDuplicatedCategories(List<int> categoryIds);
        Task<bool> NameMustBeUnique(string name, CancellationToken cancellationToken);
    }
}
