using Microsoft.AspNetCore.Mvc;
using YT_Downloader.Services;

namespace YT_Downloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly DownloadHandler _downloadHandler;

        public DownloadController(DownloadHandler downloadHandler)
        {
            _downloadHandler = downloadHandler;
        }

        [HttpGet("stream")]
        public async Task<IActionResult> StreamVideo([FromQuery] string url, [FromQuery] bool audioOnly = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("URL parameter is required");
            }

            var title = await _downloadHandler.GetPageTitleAsync(url);

            var extension = audioOnly ? "m4a" : "mp4";
            var encodedFileName = Uri.EscapeDataString($"{title}.{extension}");

            // Set response headers for streaming
            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{encodedFileName}\"");
            Response.Headers.Append("Content-Type", audioOnly ? "audio/m4a" : "video/mp4");

            var stream = await _downloadHandler.GetDataStream(url, title, audioOnly);
            await stream.CopyToAsync(Response.Body).ConfigureAwait(false);

            return new EmptyResult();
        }
    }
}
