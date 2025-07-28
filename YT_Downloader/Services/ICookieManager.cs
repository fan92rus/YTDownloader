namespace YT_Downloader.Services
{
    public interface ICookieManager
    {
        Task<string> LoadCookiesAsync();
        Task SaveCookiesAsync(string cookieContent);
        string GetCookiePath();
    }
}
