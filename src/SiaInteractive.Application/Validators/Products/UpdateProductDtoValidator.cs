using FluentValidation;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Infraestructure.Interfaces;

namespace SiaInteractive.Application.Validators.Products
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateProductDtoValidator(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Id)
                .NotNull().GreaterThan(0).WithMessage("Product Id is required.")
                .MustAsync(ProductMustExist).WithMessage("Product does not exist.");

            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage("Product name is required.")
                    .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.")
                        .MustAsync(async (dto, name, ct) => await NameMustBeUnique(name, dto, ct)).WithMessage("Product name already being used");

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

        private async Task<bool> ProductMustExist(int productId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(productId);

            return product != null;
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

        private async Task<bool> NameMustBeUnique(string name, UpdateProductDto dto, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(name))
                return true;
            var existingName = await _productRepository.ExistingNameAsync(name, dto.Id);
            return !existingName;
        }
    }
}
