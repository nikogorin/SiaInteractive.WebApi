using FluentValidation;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Infraestructure.Interfaces;

namespace SiaInteractive.Application.Validators.Products
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public CreateProductDtoValidator(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage("Product name is required.")
                    .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.")
                        .MustAsync(NameMustBeUnique).WithMessage("Product name already being used");
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Product description must not exceed 1000 characters.");
            RuleFor(x => x.Image)
                .Must(BeValidBase64).WithMessage("Image must be a valid Base64 string.")
                    .When(x => !string.IsNullOrEmpty(x.Image));
            RuleFor(x => x.CategoryIds)
                .NotNull().WithMessage("Category is required.")
                .NotEmpty().WithMessage("At least one category is required.")
                    .Must(NotDuplicatedCategories).WithMessage("One or more categories are duplicated.")
                    .MustAsync(AllCategoriesExist).WithMessage("One or more categories do not exist.");
        }

        private bool BeValidBase64(string? image)
        {
            var commaIndex = image!.IndexOf(',');
            if (commaIndex >= 0)
                image = image[(commaIndex + 1)..];

            var buffer = new Span<byte>(new byte[image.Length]);
            return Convert.TryFromBase64String(image, buffer, out _);
        }

        private async Task<bool> AllCategoriesExist(List<int> categoryIds, CancellationToken cancellationToken)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return true;

            var existingCount = await _categoryRepository.CountExistingIdsAsync(categoryIds);
            return existingCount == categoryIds.Count;
        }

        private bool NotDuplicatedCategories(List<int> categoryIds)
        {
            
            if (categoryIds == null || categoryIds.Count == 0)
                return true;

            return categoryIds.Distinct().Count() == categoryIds.Count;
        }

        private async Task<bool> NameMustBeUnique(string name, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(name))
                return true;

            var existingName = await _productRepository.ExistingNameAsync(name);
            return !existingName;
        }
    }
}