namespace YT_Downloader.Services
{
    public class CookieManager : ICookieManager
    {
        private readonly string _cookiePath;

        public CookieManager()
        {
            _cookiePath = Environment.GetEnvironmentVariable("COOKIE_PATH") ?? "./data/cookie.txt";
        }

        public async Task<string> LoadCookiesAsync()
        {
            try
            {
                if (File.Exists(_cookiePath))
                {
                    return await File.ReadAllTextAsync(_cookiePath);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки кук: {ex.Message}");
            }
        }

        public async Task SaveCookiesAsync(string cookieContent)
        {
            try
            {
                await File.WriteAllTextAsync(_cookiePath, cookieContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения кук: {ex.Message}");
            }
        }

        public string GetCookiePath()
        {
            return _cookiePath;
        }
    }
}
