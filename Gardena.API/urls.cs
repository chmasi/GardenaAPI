using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.NET
{
    public static class urls
    {
        public static string GetSessionsUrl => $"{UrlBase}{UrlGetSessions}";
        public static string GetLocationsUrl => $"{UrlBase}{UrlGetLocations}";
        public static string GetDevicesUrl => $"{UrlBase}{UrlGetDevices}";
        public static string SendCommandUrl => $"{UrlBase}{UrlSendCommand}";


        public static string UrlBase = "https://sg-api.dss.husqvarnagroup.net/sg-1";
        public static string UrlGetSessions = "/sessions";
        public static string UrlGetLocations = "/locations/?user_id=";
        public static string UrlGetDevices = "/devices?locationId=";
        public static string UrlSendCommand = "/devices/";
    }
}
