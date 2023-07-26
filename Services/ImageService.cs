using ContactHarbor.Services.Interfaces;

namespace ContactHarbor.Services;

public class ImageService : IImageService
{
    private readonly string defaultImage = "/img/DefaultContactImage.png";
    
    public string ConvertByteArrayToFile(byte[] fileData, string fileExtension)
    {
        if (fileData is null) return defaultImage;
        string imageBase64Data = Convert.ToBase64String(fileData);
        return $"data:{fileExtension};base64,{imageBase64Data}";
    }

    public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
    {
        MemoryStream memoryStream = new();
        await file.CopyToAsync(memoryStream);
        byte[] byteFile = memoryStream.ToArray();
        memoryStream.Close();
        memoryStream.Dispose();
        return byteFile;
    }
}
