using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace YT_Downloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly string _videoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");

        [HttpGet("stream")]
        public async Task<IActionResult> StreamVideo([FromQuery] string url, [FromQuery] bool audioOnly = false)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("URL parameter is required");
            }

            var client = new HttpClient();
            var page = client.GetAsync(url).Result;
            string htmlContent = await page.Content.ReadAsStringAsync();

            string title = string.Empty;
            int titleStartIndex = htmlContent.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
            if (titleStartIndex != -1)
            {
                int titleEndIndex =
                    htmlContent.IndexOf("</title>", titleStartIndex, StringComparison.OrdinalIgnoreCase);
                if (titleEndIndex != -1)
                {
                    title = htmlContent.Substring(titleStartIndex + "<title>".Length,
                        titleEndIndex - (titleStartIndex + "<title>".Length));
                }
            }

            Console.WriteLine($"Page Title: {title}");

            var cookiePath = Environment.GetEnvironmentVariable("COOKIE_PATH") ?? "/app/data/cookie.txt";

            try
            {
                string extension = audioOnly ? "m4a" : "mp4";
                string encodedFileName = Uri.EscapeDataString($"{title}.{extension}");
                string formatSelector = audioOnly ? "bestaudio[ext=m4a]" : "best[ext=mp4]";

                // Set response headers for streaming
                Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{encodedFileName}\"");
                Response.Headers.Append("Content-Type", audioOnly ? "audio/m4a" : "video/mp4");

                // Start yt-dlp process and stream directly to HTTP response
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "yt-dlp",
                        Arguments = $"{url} -o - --cookies=\"{cookiePath}\" -f \"{formatSelector}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                // Pipe yt-dlp output directly to HTTP response
                await using (var stdout = process.StandardOutput.BaseStream)
                {
                    await stdout.CopyToAsync(Response.Body).ConfigureAwait(false);
                }

                return new EmptyResult(); // Response already sent via streaming
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}