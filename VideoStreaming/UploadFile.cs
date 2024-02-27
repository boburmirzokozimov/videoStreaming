namespace VideoStreaming;

public class UploadFile
{

    public UploadFile(Stream stream, string path,string fileName, long size, string contentType)
    {
        Stream = stream;
        Path = path;
        FileName = fileName;
        Size = size;
        ContentType = contentType;
    }
    public Stream Stream { get; }
    public string Path { get; set; }
    public string FileName { get; }
    public long Size { get; }
    public string ContentType { get; }
}