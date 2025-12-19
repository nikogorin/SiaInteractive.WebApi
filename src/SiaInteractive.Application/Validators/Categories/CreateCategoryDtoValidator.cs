using FluentValidation;
using SiaInteractive.Application.Dtos.Categories;
using SiaInteractive.Application.Interfaces;

namespace SiaInteractive.Application.Validators.Categories
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        private readonly ICategoryValidatorService _categoryValidatorService;
        public CreateCategoryDtoValidator(ICategoryValidatorService categoryValidatorService)
        {
            _categoryValidatorService = categoryValidatorService;

            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage("Category name is required.")
                    .MaximumLength(200).WithMessage("Category name must not exceed 200 characters.")
                        .MustAsync(_categoryValidatorService.NameMustBeUnique).WithMessage("Category name already being used.");
        }
    }
}
