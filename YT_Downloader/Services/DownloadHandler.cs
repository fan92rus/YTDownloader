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

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL parameter is required");
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp",
                    Arguments = $"{url} --get-filename -o \"%(title)s\"",
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
                        Arguments = $"{url} -o - --cookies=\"{cookiePath}\" -f \"{formatSelector}\"",
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
