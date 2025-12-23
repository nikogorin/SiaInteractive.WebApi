using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SiaInteractive.Abstractions.Interfaces;

namespace SiaInteractive.Infraestructure.Storage
{
    public class FileStorage : IFileStorage
    {
        private readonly FileStorageOptions _options;
        private readonly ILogger<FileStorage> _logger;

        public FileStorage(IOptions<FileStorageOptions> options, ILogger<FileStorage> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<string> SaveProductImageAsync(Stream contentFile, string originalFileName, CancellationToken cancellationToken)
        {
            //var uploadPath = Path.Combine(rootPath, uploadFolder);

            //if (!Directory.Exists(uploadPath))
            //    Directory.CreateDirectory(uploadPath);

            //var extension = Path.GetExtension(file.FileName);
            //var fileName = $"{Guid.NewGuid()}{extension}";
            //var physicalPath = Path.Combine(uploadPath, fileName);

            //await using (var stream = new FileStream(physicalPath, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream, cancellationToken);
            //}

            //return $"/{uploadFolder}/{fileName}";
            var uploadPath = Path.Combine(_options.RootPath, _options.UploadFolder);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var extension = Path.GetExtension(originalFileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var physicalPath = Path.Combine(uploadPath, fileName);

            await using var stream = new FileStream(physicalPath, FileMode.Create);
            await contentFile.CopyToAsync(stream, cancellationToken);

            return $"/{_options.UploadFolder.Trim('/')}/{fileName}";
        }

        public Task DeleteIfExistsAsync(string? publicPath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(publicPath))
                return Task.CompletedTask;

            try
            {
                var fileName = Path.GetFileName(publicPath);
                var physicalPath = Path.Combine(_options.RootPath, _options.UploadFolder, fileName);

                if (File.Exists(physicalPath))
                    File.Delete(physicalPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete image file. Path: {PublicPath}", publicPath);
            }

            return Task.CompletedTask;
        }
    }
}
