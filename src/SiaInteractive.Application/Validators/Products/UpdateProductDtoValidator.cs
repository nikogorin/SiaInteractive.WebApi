using FluentValidation;
using SiaInteractive.Application.Common;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Application.Interfaces;

namespace SiaInteractive.Application.Validators.Products
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        private readonly IProductValidatorService _productValidatorService;
        private readonly ICategoryValidatorService _categoryValidatorService;

        public UpdateProductDtoValidator(IProductValidatorService productValidatorService, ICategoryValidatorService categoryValidatorService)
        {
            _productValidatorService = productValidatorService;
            _categoryValidatorService = categoryValidatorService;

            RuleFor(x => x.Id)
                .NotNull().GreaterThan(0).WithMessage("Product Id is required.")
                .MustAsync(_productValidatorService.ProductMustExist).WithMessage("Product does not exist.");

            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage("Product name is required.")
                    .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.")
                        .MustAsync(async (dto, name, ct) => await _productValidatorService.NameMustBeUnique(name, dto, ct)).WithMessage("Product name already being used");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Product description must not exceed 1000 characters.");
            RuleFor(x => x.Image)
                .Must(Base64Helper.BeValidBase64).WithMessage("Image must be a valid Base64 string.")
                    .When(x => !string.IsNullOrEmpty(x.Image));
            RuleFor(x => x.CategoryIds)
                .NotNull().WithMessage("Category is required.")
                .NotEmpty().WithMessage("At least one category is required.")
                    .Must(_categoryValidatorService.NotDuplicatedCategories).WithMessage("One or more categories are duplicated.")
                    .MustAsync(_categoryValidatorService.AllCategoriesExist).WithMessage("One or more categories do not exist.");
        }
    }
}
