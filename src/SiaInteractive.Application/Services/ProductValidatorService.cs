using SiaInteractive.Abstractions.Interfaces;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Application.Interfaces;

namespace SiaInteractive.Application.Services
{
    public class ProductValidatorService : IProductValidatorService
    {
        private readonly IProductRepository _productRepository;

        public ProductValidatorService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> ProductMustExist(int productId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(productId, cancellationToken);

            return product != null;
        }

        public async Task<bool> NameMustBeUnique(string name, UpdateProductDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;
            var existingName = await _productRepository.ExistingNameAsync(name, cancellationToken, dto.Id);
            return !existingName;
        }

        public async Task<bool> NameMustBeUnique(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            var existingName = await _productRepository.ExistingNameAsync(name, cancellationToken);
            return !existingName;
        }
    }
}
