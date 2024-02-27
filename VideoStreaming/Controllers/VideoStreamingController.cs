using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.AspNetCore.Mvc;

namespace VideoStreaming.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoStreamingController : ControllerBase
{
    [HttpPost(Name = "GetVideoStreaming")]
    public async Task<IActionResult> PostAsync([FromForm] VideoRequest request)
    {
        var file = request.File.ToUploadFile();
        var extension = Path.GetExtension(file.Path);
        var fileName = Guid.NewGuid() + Path.GetExtension(extension);

        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(uploadDir))
        {
            Directory.CreateDirectory(uploadDir);
        }

        var filePath = Path.Combine(uploadDir, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }
        var outputPlaylistPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "output.m3u8");

        await FFMpegArguments
            .FromFileInput(filePath)
            .OutputToFile(outputPlaylistPath, true, options => options
                    .WithAudioCodec(AudioCodec.Aac)
                    .WithVideoCodec(VideoCodec.LibX264)
                    .WithCustomArgument("-hls_time 10")
                    .WithCustomArgument("-hls_list_size 0")
                    .WithCustomArgument(
                        $"-hls_segment_filename {Path.Combine(uploadDir, "segment_%03d.ts")}")
            )
            .ProcessAsynchronously();

        return Ok();
    }

    [HttpGet("video/{segmentName}")]
    public IActionResult Get(string segmentName)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var filesDirectory = Path.Combine(currentDirectory, "Uploads");
        var filePath = Path.Combine(filesDirectory, segmentName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        return PhysicalFile(filePath, "application/octet-stream");
    }
}