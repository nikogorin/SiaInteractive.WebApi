namespace SiaInteractive.Application.Common
{
    public static class Base64Helper
    {
        public static bool BeValidBase64(string image)
        {
            image = RemoveHeader(image);

            var buffer = new Span<byte>(new byte[image.Length]);
            return Convert.TryFromBase64String(image, buffer, out _);
        }

        public static bool BeWithinSizeLimit(string image, long maxFileSizeBytes)
        {
            var sizeInBytes = GetBase64SizeInBytes(image);
            return sizeInBytes <= maxFileSizeBytes;
        }

        /// <summary>
        /// remove the header if exists (data:image/png;base64,...)
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static string RemoveHeader(string image)
        {
            var commaIndex = image!.IndexOf(',');
            if (commaIndex >= 0)
                image = image[(commaIndex + 1)..];
            return image;
        }

        private static long GetBase64SizeInBytes(string image)
        {
            image = RemoveHeader(image);

            var padding = image.EndsWith("==") ? 2 :
                          image.EndsWith("=") ? 1 : 0;

            return (image.Length * 3L / 4L) - padding;
        }
    }
}
