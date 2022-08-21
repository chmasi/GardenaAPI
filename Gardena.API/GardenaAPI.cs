using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Gardena.Net.Extensions;
using Gardena.Net.Model;
using Gardena.Net.Utils;
using Newtonsoft.Json;


namespace Gardena.Net
{
    public delegate void LoginSuccessfulHandler(object sender);
    public delegate void LoginFailedlHandler(object sender);
    public delegate void RefreshTokenSuccessfulHandler(object sender);
    public delegate void RefreshTokenFailedlHandler(object sender);

    public class GardenaApi
    {

        public string ClientId { get; }
        public string ClientSecret { get; }

        public OAuthAccessToken OAuthAccessToken { get; set; }
        //public OAuthAccessToken token { get; private set; }

        private readonly HttpClient _httpClient;

        public event LoginSuccessfulHandler LoginSuccessful;
        public event LoginFailedlHandler LoginFailed;

        public event RefreshTokenSuccessfulHandler RefreshTokenSuccessful;
        public event RefreshTokenFailedlHandler RefreshTokenFailed;



        /// <summary>
        /// Create the Netatmo API
        /// </summary>
        /// <param name="clientId">application OAuth Client id, found under "https://developer.husqvarnagroup.cloud/"</param>
        /// <param name="clientSecret">application OAuth Client secret, found under "https://developer.husqvarnagroup.cloud/"</param>
        public GardenaApi(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Login to netatmo and retrieve an OAuthToken
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public async void Login(string email, string password)
        {
            var content = HttpContentCreator.CreateLoginHttpContent(ClientId, email, password);
            var response = await Request<OAuthAccessToken>(Urls.RequestTokenUrl, content, true, "POST");
            //var response = await myGardenaTokenRequest<OAuthAccessToken>();

            if (response.Success)
            {
                OAuthAccessToken = response.Result;
                OAuthAccessToken.CreationTime = response.Result._creationTime;
                OAuthAccessToken.MinusExpiresSeconds = response.Result._minusExpiresSeconds;
                OnLoginSuccessful();
            }
            else
            {
                OnLoginFailed();
            }
        }

        /// <summary>
        /// retrieve an OAuthToken by using Authorization
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="scopes"></param>
        public async void AuthorizationCode(string code, string redirect_uri, string state)
        {
            var content = HttpContentCreator.CreateAuthorizationRequestHttpContent(ClientId, ClientSecret, code, redirect_uri, state);
            var response = await Request<OAuthAccessToken>(Urls.RequestTokenUrl, content, true, "POST");
            if (response.Success)
            {
                OAuthAccessToken = response.Result;
                OAuthAccessToken.CreationTime = response.Result._creationTime;
                OAuthAccessToken.MinusExpiresSeconds = response.Result._minusExpiresSeconds;
                OnLoginSuccessful();
            }
            else
            {
                OnLoginFailed();
            }
        }

        private async Task<bool> RefreshToken()
        {
            var content = HttpContentCreator.CreateRefreshTokenHttpContent(OAuthAccessToken.RefreshToken, ClientId);
            var response = await Request<OAuthAccessToken>(Urls.RequestTokenUrl, content, true, "POST");
            if (!response.Success)
            {
                OnRefreshTokenFailed();
                return false;
            }
            else
            {
                OAuthAccessToken = response.Result;
                OAuthAccessToken.CreationTime = response.Result._creationTime;
                OAuthAccessToken.MinusExpiresSeconds = response.Result._minusExpiresSeconds;
                OnRefreshTokenSuccessful();
                return true;
            }


        }

        private async Task<Response<T>> Request<T>(string url, Dictionary<string, string> content, bool isTokeRenquest = false, string payload = "-1")
        {
            var httpContent = new MultipartFormDataContent();

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();

            HttpResponseMessage clientResponse = null;

            //Check if authenticated and refresh oauth token if needed
            if (!isTokeRenquest)
            {
                if (OAuthAccessToken == null)
                    return Response<T>.CreateUnsuccessful("Please login first", HttpStatusCode.Unauthorized);

                if (OAuthAccessToken.NeedsRefresh)
                {
                    var refreshed = await RefreshToken();
                    if (!refreshed)
                        return Response<T>.CreateUnsuccessful("Unable to refresh token", HttpStatusCode.ExpectationFailed);
                }
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OAuthAccessToken.AccessToken);
            }
            foreach (var key in content.Keys)
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(key, content[key]);
                //httpContent.Add(new StringContent(content[key]), key);
            }
            if (payload == "GET")  // get location etc
            {
                httpRequestMessage.Method = new HttpMethod("GET");
                //httpRequestMessage.Content = new FormUrlEncodedContent(content);
                httpRequestMessage.RequestUri = new Uri(url);
                clientResponse = await _httpClient.SendAsync(httpRequestMessage);
            }
            else if (payload == "POST")
            {
                httpRequestMessage.Method = new HttpMethod("POST");
                httpRequestMessage.Content = new FormUrlEncodedContent(content);
                clientResponse = await _httpClient.PostAsync(url, httpRequestMessage.Content);
            }
            else if (payload == "PUT")
            {
                httpRequestMessage.Method = new HttpMethod("PUT");
                httpRequestMessage.RequestUri = new Uri(url);
                httpRequestMessage.Content = new StringContent("{\"data\":{\"id\":\"request-1\",\"type\":\"MOWER_CONTROL\",\"attributes\":{\"command\":\"PARK_UNTIL_NEXT_TASK\"}}}");
                httpRequestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.api+json");
                clientResponse = await _httpClient.SendAsync(httpRequestMessage);
            }

            var responseString = await clientResponse.Content.ReadAsStringAsync();
            var responseResult = clientResponse.IsSuccessStatusCode ?
                Response<T>.CreateSuccessful(JsonConvert.DeserializeObject<T>(responseString), clientResponse.StatusCode) :
                Response<T>.CreateUnsuccessful(responseString, clientResponse.StatusCode);

            return responseResult;
        }

        public async Task<Response<gardena_locations>> getGardena_Locations()
        {
            var content = HttpContentCreator.CreateLocationsHttpContent(ClientId);
            var response = await Request<gardena_locations>(Urls.GetLocationsDataUrl, content, false, "GET");
            return response;
        }

        public async Task<Response<gardena_locations_id>> getGardena_Locations_Id(string locationID)
        {
            var content = HttpContentCreator.CreateLocationsIdHttpContent(ClientId);
            string URLID = Urls.GetLocationsDataUrl + "/" + locationID;
            var response = await Request<gardena_locations_id>(URLID, content, false, "GET");
            return response;
        }

        public async Task<Response<gardena_command>> putGardena_Command(string serviceID)
        {
            var content = HttpContentCreator.CreateCommandHttpContent(ClientId);
            string URLID = Urls.GetCommandUrl + "/" + serviceID;
            var response = await Request<gardena_command>(URLID, content, false, "PUT");
            return response;
        }

        public string health()
        {

            string responseString = String.Empty;
            using (var client = new HttpClient())
            {
                var url = "https://api.smart.gardena.dev/v1/health";
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    responseString = responseContent.ReadAsStringAsync().Result;

                    //Console.WriteLine(responseString);
                }
            }
            return responseString;
        }


        /* ERROR CODES
         
        0 => 'uninitialised', 1 => 'paused', 
        2 => 'ok_cutting', 
        3 => 'ok_searching', 
        4 => 'ok_charging', 
        5 => 'ok_leaving', 
        6 => 'wait_updating', 
        7 => 'wait_power_up', 
        8 => 'parked_timer', 
        9 => 'parked_park_selected', 
        10 => 'off_disabled', 
        11 => 'off_hatch_open', 
        12 => 'unknown', 
        13 => 'error', 
        14 => 'error_at_power_up', 
        15 => 'off_hatch_closed', 
        16 => 'ok_cutting_timer_overridden', 
        17 => 'parked_autotimer', 
        18 => 'parked_daily_limit_reached', 
        19 => 'undefined' 
         
         */


        /* LAST ERROR CODE
        0 => 'uninitialised', 
        1 => 'no_message', 
        2 => 'outside_working_area', 
        3 => 'no_loop_signal', 
        4 => 'wrong_loop_signal', 
        5 => 'loop_sensor_problem_front', 
        6 => 'loop_sensor_problem_rear', 
        7 => 'loop_sensor_problem_left', 
        8 => 'loop_sensor_problem_right', 
        9 => 'wrong_pin_code', 
        10 => 'trapped', 
        11 => 'upside_down', 
        12 => 'low_battery', 
        13 => 'empty_battery', 
        14 => 'no_drive', 
        15 => 'temporarily_lifted', 
        16 => 'lifted', 
        17 => 'stuck_in_charging_station', 
        18 => 'charging_station_blocked', 
        19 => 'collision_sensor_problem_rear', 
        20 => 'collision_sensor_problem_front', 
        21 => 'wheel_motor_blocked_right', 
        22 => 'wheel_motor_blocked_left', 
        23 => 'wheel_drive_problem_right', 
        24 => 'wheel_drive_problem_left', 
        25 => 'cutting_motor_drive_defect', 
        26 => 'cutting_system_blocked', 
        27 => 'invalid_sub_device_combination', 
        28 => 'settings_restored', 
        29 => 'memory_circuit_problem', 
        30 => 'slope_too_steep', 
        31 => 'charging_system_problem', 
        32 => 'stop_button_problem', 
        33 => 'tilt_sensor_problem', 
        34 => 'mower_tilted', 
        35 => 'wheel_motor_overloaded_right', 
        36 => 'wheel_motor_overloaded_left', 
        37 => 'charging_current_too_high', 
        38 => 'electronic_problem', 
        39 => 'cutting_motor_problem', 
        40 => 'limited_cutting_height_range', 
        41 => 'unexpected_cutting_height_adj', 
        42 => 'cutting_height_problem_drive', 
        43 => 'cutting_height_problem_curr', 
        44 => 'cutting_height_problem_dir', 
        45 => 'cutting_height_blocked', 
        46 => 'cutting_height_problem', 
        47 => 'no_response_from_charger', 
        48 => 'ultrasonic_problem', 
        49 => 'temporary_problem', 
        50 => 'guide_1_not_found', 
        51 => 'guide_2_not_found', 
        52 => 'guide_3_not_found', 
        53 => 'gps_tracker_module_error', 
        54 => 'weak_gps_signal', 
        55 => 'difficult_finding_home', 
        56 => 'guide_calibration_accomplished', 
        57 => 'guide_calibration_failed', 
        58 => 'temporary_battery_problem', 
        59 => 'battery_problem', 
        60 => 'too_many_batteries', 
        61 => 'alarm_mower_switched_off', 
        62 => 'alarm_mower_stopped', 
        63 => 'alarm_mower_lifted', 
        64 => 'alarm_mower_tilted', 
        65 => 'alarm_mower_in_motion', 
        66 => 'alarm_outside_geofence', 
        67 => 'connection_changed', 
        68 => 'connection_not_changed', 
        69 => 'com_board_not_available', 
        70 => 'slipped', 
        71 => 'invalid_battery_combination', 
        72 => 'imbalanced_cutting_disc', 
        73 => 'safety_function_faulty'    
        


        0 => 'uninitialised', 
        1 => 'no_source', 
        2 => 'completed_cutting_daily_limit', 
        3 => 'week_timer', 
        4 => 'countdown_time', 
        5 => 'mower_charging', 
        6 => 'completed_cutting_autotimer',
        7 => 'undefined'
         
         */

        protected virtual void OnLoginSuccessful()
        {
            LoginSuccessful?.Invoke(this);
        }

        protected virtual void OnLoginFailed()
        {
            LoginFailed?.Invoke(this);
        }

        protected virtual void OnRefreshTokenSuccessful()
        {
            RefreshTokenSuccessful?.Invoke(this);
        }

        protected virtual void OnRefreshTokenFailed()
        {
            RefreshTokenFailed?.Invoke(this);
        }
    }
}