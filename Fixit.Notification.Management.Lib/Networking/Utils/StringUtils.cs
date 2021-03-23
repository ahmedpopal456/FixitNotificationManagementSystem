
namespace Fixit.Notification.Management.Lib.Networking.Utils
{
    public static class StringUtils
    {
        public static string TrimCharacter(string iString, char iCharToTrim = '/')
        {
            return iString.Trim(iCharToTrim);
        }
        public static string ToAzureDirectoryPath(string iDirectory)
        {
            return iDirectory.Replace('\\', '/');
        }
    }
}
