namespace TheBlogProject.Services;

public interface IImageService
{
    Task<byte[]> EncodeImageAsync(IFormFile file);
    Task<byte[]> EncodeImageAsync(string fileName);
    string DecodeImage(byte[] data, string type);
    string ImageType(IFormFile file);
    int Size(IFormFile file);
}