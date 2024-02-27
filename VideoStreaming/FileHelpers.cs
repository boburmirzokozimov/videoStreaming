using Microsoft.Net.Http.Headers;

namespace VideoStreaming;

public static class FileHelpers
{
    public static UploadFile ToUploadFile(this IFormFile? formFile)
    {
        var parsed = ContentDispositionHeaderValue.Parse(formFile!.ContentDisposition);

        return new UploadFile(
            stream: formFile.OpenReadStream(),
            path: parsed.FileName.Trim().ToString(),
            fileName: parsed.Name.Trim().ToString(),
            size: formFile.Length,
            contentType: formFile.ContentType);
    }
}
