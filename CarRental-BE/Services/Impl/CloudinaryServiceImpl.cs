
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

        public async Task<string> DeleteImageAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result == "ok" || deletionResult.Result == "not found")
            {
                return "Deleted";
            }

            throw new Exception($"Failed to delete image: {deletionResult.Error?.Message}");
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file uploaded.", nameof(file));

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                AssetFolder = folderName
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
