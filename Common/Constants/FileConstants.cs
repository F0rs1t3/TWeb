namespace TWeb.Common.Constants
{
    public static class FileConstants
    {
        public const int MaxFileSize = 5 * 1024 * 1024; // 5MB
        
        public static readonly string[] AllowedImageExtensions = 
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
        };
    }
}