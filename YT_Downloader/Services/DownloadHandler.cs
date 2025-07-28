using System.Diagnostics;
using System.Text;

namespace YT_Downloader.Services
{
    public class DownloadHandler
    {
        private readonly ICookieManager _cookieService;

        public DownloadHandler(ICookieManager cookieService)
        {
            _cookieService = cookieService;
        }

        public async Task<string> GetPageTitleAsync(string url)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Invalid URL format. Only HTTP and HTTPS URLs are supported.");
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp",
                    ArgumentList = { $"{url}", "--get-filename", "-o", "\"%(title)s\"" },
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();

            Console.WriteLine($"Page Title: {output.Trim()}");
            return output.Trim();
        }

        public async Task<Stream> GetDataStream(string url, string title, bool audioOnly)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Invalid URL format. Only HTTP and HTTPS URLs are supported.");
            }

            var cookiePath = _cookieService.GetCookiePath();

            try
            {
                var formatSelector = audioOnly ? "bestaudio[ext=m4a]" : "best[ext=mp4]";

                // Start yt-dlp process and stream directly to HTTP response
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "yt-dlp",
                        ArgumentList = { $"{url}", "-o", "-", $"--cookies={cookiePath}", "-f", formatSelector },
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                // Pipe yt-dlp output directly to HTTP response
                return process.StandardOutput.BaseStream;
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex.Message}");
            }
        }
    }
}
