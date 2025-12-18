using FluentValidation;
using SiaInteractive.Application.Dtos.Categories;
using SiaInteractive.Infraestructure.Interfaces;

namespace SiaInteractive.Application.Validators.Categories
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        public CreateCategoryDtoValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage("Category name is required.")
                    .MaximumLength(200).WithMessage("Category name must not exceed 200 characters.")
                        .MustAsync(NameMustBeUnique).WithMessage("Category name already being used.");
        }

        private async Task<bool> NameMustBeUnique(string name, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(name))
                return true;

            var existingName = await _categoryRepository.ExistingNameAsync(name);
            return !existingName;
        }
    }
}
