using System;
using System.Collections.Generic;
using System.Net.Http;
using Gardena.Net.Extensions;
using Gardena.Net.Model;
using Newtonsoft.Json;

namespace Gardena.Net.Utils
{
    public class HttpContentCreator
    {
        public static Dictionary<string, string> CreateLoginHttpContent(string clientId, string email, string password)
        {
            var content = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"Content-Type", "application/x-www-form-urlencoded"},
                {"client_id", clientId},
                {"username", email},
                {"password", password}
            };

            return content;
        }

        public static Dictionary<string, string> CreateAuthorizationRequestHttpContent(string clientId, string clientSecret, string code, string redirect_uri, string state)
        {
            var content = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"code", code},
                {"redirect_uri", redirect_uri},
                {"state", state }
            };
            return content;
        }

        public static Dictionary<string, string> CreateRefreshTokenHttpContent(string refreshToken, string clientId)
        {
            var content = new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token"},
                    {"content-type", "application/x-www-form-urlencoded"},
                    {"refresh_token", refreshToken },
                    {"client_id",  clientId}
                };
            return content;
        }

        public static Dictionary<string, string> CreateLocationsHttpContent(string clientId)
        {
            var content = new Dictionary<string, string>
                {
                    {"accept","application/vnd.api+json"},
                    {"X-Api-Key", clientId },
                    {"Authorization-Provider","husqvarna"},
                };
            return content;
        }

        public static Dictionary<string, string> CreateLocationsIdHttpContent(string clientId)
        {
            var content = new Dictionary<string, string>
                {
                    {"accept","application/vnd.api+json"},
                    {"X-Api-Key", clientId },
                    {"Authorization-Provider","husqvarna"},
                };
            return content;
        }

        public static Dictionary<string, string> CreateCommandHttpContent(string clientId)
        {
            var content = new Dictionary<string, string>
                {
                    {"accept", "*/*"},
                    {"X-Api-Key", clientId },
                    {"Authorization-Provider","husqvarna"},
                };
            return content;
        }

    }
}
