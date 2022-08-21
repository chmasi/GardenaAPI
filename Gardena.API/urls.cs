namespace Gardena.Net
{
    public static class Urls
    {
        public static string RequestAuthorizationUrl => $"{UrlBase}{UrlAuthorization}";
        public static string RequestTokenUrl => $"{UrlBase}{UrlRequestTokenPath}";
        public static string GetLocationsDataUrl => $"{GardenaBase}{UrlGetLocationsData}";
        public static string GetCommandUrl => $"{GardenaBase}{UrlPutCommand}";
        public static string GetPublicDataUrl => $"{GardenaBase}{UrlGetPublicData}";

        public static string UrlBase = "https://api.authentication.husqvarnagroup.dev/v1";
        public static string GardenaBase = "https://api.smart.gardena.dev/v1";
        public static string UrlAuthorization = "/oauth2/authorize";
        public static string UrlRequestTokenPath = "/oauth2/token";
        public static string UrlGetLocationsData = "/locations";
        public static string UrlPutCommand = "/command";
        public static string UrlGetPublicData = "/api/getpublicdata";
    }
}


