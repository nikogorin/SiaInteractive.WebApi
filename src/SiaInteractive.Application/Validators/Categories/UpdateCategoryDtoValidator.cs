using FluentValidation;
using SiaInteractive.Application.Dtos.Categories;
using SiaInteractive.Infraestructure.Interfaces;

namespace SiaInteractive.Application.Validators.Categories
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryDtoValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(RuleFor => RuleFor.Id)
                .NotNull().WithMessage("Category Id is required.")
                    .GreaterThan(0).WithMessage("Category Id must be greater than zero.");
            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage("Category Name is required.")
                    .MaximumLength(200).WithMessage("Category Name must not exceed 200 characters.")
                        .MustAsync(async (dto, name, ct) => await NameMustBeUnique(name, dto, ct)).WithMessage("Category name already being used.");
        }

        private async Task<bool> NameMustBeUnique(string name, UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            var existingName = await _categoryRepository.ExistingNameAsync(name, dto.Id);
            return !existingName;
        }
    }
}
