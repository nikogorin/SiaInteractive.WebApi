namespace SiaInteractive.Infraestructure.Storage
{
    public sealed class FileStorageOptions
    {
        public string RootPath { get; set; } = default!;
        public string UploadFolder { get; set; } = "uploads";
    }
}