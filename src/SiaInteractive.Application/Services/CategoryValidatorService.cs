using SiaInteractive.Abstractions.Interfaces;
using SiaInteractive.Application.Interfaces;

namespace SiaInteractive.Application.Services
{
    public class CategoryValidatorService : ICategoryValidatorService
    {
        private readonly ICategoryRepository _categoryRepository;
        
        public CategoryValidatorService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> AllCategoriesExist(List<int> categoryIds, CancellationToken cancellationToken)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return true;

            var existingCount = await _categoryRepository.CountExistingIdsAsync(categoryIds, cancellationToken);
            return existingCount == categoryIds.Count;
        }

        public bool NotDuplicatedCategories(List<int> categoryIds)
        {

            if (categoryIds == null || categoryIds.Count == 0)
                return true;

            return categoryIds.Distinct().Count() == categoryIds.Count;
        }

        public async Task<bool> NameMustBeUnique(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            var existingName = await _categoryRepository.ExistingNameAsync(name, cancellationToken);
            return !existingName;
        }
    }
}
