
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace CarRental_BE.Services.Impl
{
    public class CloudinaryServiceImpl : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryServiceImpl(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public Task<string> DeleteImageAsync(string publicId)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> TestUpload()
        {
            string imagePath = "C:\\Users\\Legion\\OneDrive\\Pictures\\Screenshots\\Screenshot 2025-06-09 215804.png";
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                throw new ArgumentException("Image path is invalid or file does not exist.", nameof(imagePath));

            await using var stream = File.OpenRead(imagePath);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(Path.GetFileName(imagePath), stream),
                AssetFolder = "CarRental",
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }
            else
            {
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error?.Message}");
            }
        }
    }
}
