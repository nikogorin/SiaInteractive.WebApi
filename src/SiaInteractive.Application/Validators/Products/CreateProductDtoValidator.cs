using FluentValidation;
using SiaInteractive.Application.Common;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Application.Interfaces;

namespace SiaInteractive.Application.Validators.Products
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        private readonly ICategoryValidatorService _categoryValidatorService;
        private readonly IProductValidatorService _productValidatorService;

        private const long maxFileSizeBytes = 2 * 1024 * 1024; // 2 MB

        public CreateProductDtoValidator(ICategoryValidatorService categoryValidatorService, IProductValidatorService productValidatorService)
        {
            _categoryValidatorService = categoryValidatorService;
            _productValidatorService = productValidatorService;

            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage("Product name is required.")
                    .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.")
                        .MustAsync(_productValidatorService.NameMustBeUnique).WithMessage("Product name already being used");
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Product description must not exceed 1000 characters.");
            RuleFor(x => x.CategoryIds)
                .NotNull().WithMessage("Category is required.")
                .NotEmpty().WithMessage("At least one category is required.")
                    .Must(_categoryValidatorService.NotDuplicatedCategories).WithMessage("One or more categories are duplicated.")
                    .MustAsync(_categoryValidatorService.AllCategoriesExist).WithMessage("One or more categories do not exist.");
        }
    }
}