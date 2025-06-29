namespace CarRental_BE.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName);
        Task<string> DeleteImageAsync(string publicId);

        Task<string> TestUpload();
    }
}
