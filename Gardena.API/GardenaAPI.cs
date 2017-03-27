using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Gardena.NET.Models;

namespace Gardena.NET
{
    public delegate void LoginSuccessfulHandler(object sender);
    public delegate void LoginFailedlHandler(object sender);
    public class GardenaAPI
    {
        public Login MyLogin = new Login();

        public event LoginSuccessfulHandler LoginSuccessful;
        public event LoginFailedlHandler LoginFailed;

        #region syncronious functions
        /// <summary>
        /// Login to Gardena and retrieve token and user_id
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Login Data</returns>
        public Models.Login GardenaLogin(string email, string password)
        {
            try
            {
                //string url = "https://sg-api.dss.husqvarnagroup.net/sg-1/sessions";
                string url = urls.GetSessionsUrl;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                MyLogin.sessions = new Models.Credentials { email = email, password = password };
                string myRequestString = JsonConvert.SerializeObject(MyLogin);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(myRequestString);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string result = streamReader.ReadToEnd();
                        //gardenaLogin = new JavaScriptSerializer().Deserialize<Login>(result);
                        MyLogin = JsonConvert.DeserializeObject<Models.Login>(result);
                        MyLogin.sessions.status_message = "Ok";
                    }
                }
                OnLoginSuccessful();
            }
            catch (Exception exp)
            {
                MyLogin.sessions.status_message = exp.Message;
                OnLoginFailed();
            }
            return MyLogin;
        }

        /// <summary>
        /// Retrieve Location Data
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns>Location Data</returns>
        public Locations GetGardenaLocation(string userId, string token)
        {
            var myLocations = new Locations();
            var myResponseCode = new HttpStatusCode();

            // string url = "https://sg-api.dss.husqvarnagroup.net/sg-1/locations/?user_id=" + userId;
            string url = urls.GetLocationsUrl + userId;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("X-Session:" + token);
            httpWebRequest.Method = "GET";
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

            try
            {
                var myresponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string responseString = new StreamReader(myresponse.GetResponseStream()).ReadToEnd();              
                myLocations = JsonConvert.DeserializeObject<Locations>(responseString);
                myLocations.status_message = myresponse.StatusCode.ToString();
            }
            catch (Exception exp)
            {
                // HTTPError 404 not found => schedule does not exist
                myResponseCode = ((System.Net.HttpWebResponse)((System.Net.WebException)exp).Response).StatusCode;
                myLocations.status_message = myResponseCode.ToString();
            }
            return myLocations;
        }

        /// <summary>
        /// Returns details over all Device Configurations
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Devices GetGardenaDevices(string locationId, string token)
        {
            var myDevices = new Devices();
            var myResponseCode = new HttpStatusCode();

            //string url = "https://sg-api.dss.husqvarnagroup.net/sg-1/devices?locationId=" + locationId;
            string url = urls.GetDevicesUrl + locationId;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("X-Session:" + token);
            httpWebRequest.Method = "GET";

            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
            try
            {
                var myresponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string responseString = new StreamReader(myresponse.GetResponseStream()).ReadToEnd();
                myDevices = JsonConvert.DeserializeObject<Devices>(responseString);
                myDevices.status_message = myresponse.StatusCode.ToString();
            }
            catch (Exception exp)
            {
                // HTTPError
                myResponseCode = ((System.Net.HttpWebResponse)((System.Net.WebException)exp).Response).StatusCode;
                myDevices.status_message = myResponseCode.ToString();
            }
            return myDevices;
        }

        /// <summary>
        /// command: park_until_next_timer => Parken bis zum nächsten Zeitplan
        /// command: park_until_further_notice => Parken und Zeitplan pausieren
        /// command: start_resume_schedule => Parken und Zeitplan pausieren
        /// command: start_override_timer parameter: 1440 => 24 Stunden mähen
        /// command: start_override_timer parameter: 4320 => 3 Tage mähen
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="token"></param>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public HttpStatusCode GardenaSendCommand(string deviceID, string locationID, string token, string command, int parameter = 0)
        {
            var myResponseCode = new HttpStatusCode();
            string url = urls.SendCommandUrl + deviceID + "/abilities/mower/command?locationId=" + locationID;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("X-Session:" + token);
            httpWebRequest.Method = "POST";

            var myCommand = new Models.SendCommand();
            var myParam = new Models.Parameters();
            myParam.duration = parameter;
            myCommand.name = command;
            myCommand.parameters = myParam;
            string myRequestString = JsonConvert.SerializeObject(myCommand);

            if (myRequestString.Contains(value: "start_resume_schedule"))
            {
                myRequestString = "{ \"name\":\"start_resume_schedule\"}";
            }

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(myRequestString);
                streamWriter.Flush();
                streamWriter.Close();
                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    myResponseCode = httpResponse.StatusCode;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string result = streamReader.ReadToEnd();
                        //gardenaLogin = new JavaScriptSerializer().Deserialize<Login>(result);
                        //myCommand = JsonConvert.DeserializeObject<Models.SendCommand>(result);
                        //myCommand.status_message = myResponseCode.ToString();
                    }
                }
                catch (Exception exp)
                {
                    myResponseCode = ((System.Net.HttpWebResponse)((System.Net.WebException)exp).Response).StatusCode;
                   // myCommand.status_message = myResponseCode.ToString();
                }
            }
            return myResponseCode;
        }

        /// <summary>
        /// Creates a new Schedule
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="token"></param>
        /// <param name="startat">14:00</param>
        /// <param name="endat">16:00</param>
        /// <param name="type">active</param>
        /// <param name="recType">weekly</param>
        /// <param name="recWeekdays">Sunday, Saturday, Monday (List of week days)</param>
        /// <returns></returns>
        public Models.Schedule GardenaCreateSchedule(string deviceID, string locationID, string token,string startat, string endat, string type, string recType, List<string> recWeekdays)
        {
            //string url = "https://sg-api.dss.husqvarnagroup.net/sg-1/devices/" + deviceID + "/scheduled_events?locationId=" + locationID;
            string url = urls.SendCommandUrl + deviceID + "/scheduled_events?locationId=" + locationID;
            
            Models.Schedule myCreatedScheduleResult = null;
            // List<string> weekdays = new List<string>();
            var myResponseCode = new HttpStatusCode(); ;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("X-Session:" + token);
            httpWebRequest.Method = "POST";

            var myNewSchedule = new CreateSchedule();
            var myNewScheduledEvent = new CreateScheduledEvents();
            var myReccurence = new CreateRecurrence();

            //string uniquID = Guid.NewGuid().ToString("D");
            //weekdays.Add("sunday");
            myReccurence.type = recType;
            myReccurence.weekdays = recWeekdays;

            //myNewScheduledEvent.id = Guid.NewGuid().ToString("D");
            myNewScheduledEvent.start_at = startat;
            myNewScheduledEvent.end_at = endat;
            myNewScheduledEvent.type = type;
            myNewScheduledEvent.device = deviceID;
            myNewScheduledEvent.recurrence = myReccurence;

            myNewSchedule.scheduled_events = myNewScheduledEvent;

            string myRequestString = JsonConvert.SerializeObject(myNewSchedule);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(myRequestString);
                streamWriter.Flush();
                streamWriter.Close();
                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    myResponseCode = httpResponse.StatusCode;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string result = streamReader.ReadToEnd();
                        myCreatedScheduleResult = JsonConvert.DeserializeObject<Models.Schedule>(result);
                    }
                }
                catch (Exception exp)
                {
                    myResponseCode = ((System.Net.HttpWebResponse)((System.Net.WebException)exp).Response).StatusCode;
                }
            }
            return myCreatedScheduleResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="scheduledeventid"></param>
        /// <param name="locationID"></param>
        /// <param name="token"></param>
        /// <returns>HttpWebResponse</returns>
        public HttpStatusCode GardenaDeleteSchedule(string deviceID, string scheduledeventid, string locationID, string token)
        {
            var myResponseCode = new HttpStatusCode();
            //string url = "https://sg-api.dss.husqvarnagroup.net/sg-1/devices/" + deviceID + "/scheduled_events/" + scheduledeventid + "?locationId=" + locationID;
            string url = urls.SendCommandUrl + deviceID + "/scheduled_events/" + scheduledeventid + "?locationId=" + locationID;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("X-Session:" + token);
            httpWebRequest.Method = "DELETE";
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

            try
            {
                var myresponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string responseString = new StreamReader(myresponse.GetResponseStream()).ReadToEnd();
                myResponseCode = myresponse.StatusCode;
                SendCommand myResult = JsonConvert.DeserializeObject<Models.SendCommand>(responseString);
            }
            catch (Exception exp)
            {
                // HTTPError 404 not found => schedule not existing
                myResponseCode = ((System.Net.HttpWebResponse)((System.Net.WebException)exp).Response).StatusCode;
                //myDevices.status_message = exp.Message;
            }
            return myResponseCode;
        }


        #endregion

        #region Asyncron Procedures
        /// <summary>
        /// AsyncLogin to Gardena and retrieve token and user_id
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async void GardenaLoginAsync(string email, string password)
        {
            HttpClient myHttpClient = new HttpClient();
 
            MyLogin.sessions = new Credentials { email = email, password = password };
            var myRequestString = JsonConvert.SerializeObject(MyLogin);
            var response = myHttpClient.PostAsync(urls.GetSessionsUrl, new StringContent(myRequestString.ToString(), Encoding.UTF8, "application/json")).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                MyLogin = JsonConvert.DeserializeObject<Login>(result);
                MyLogin.sessions.status_message = response.StatusCode.ToString();
                MyLogin.sessions.email = email;
                MyLogin.sessions.password = password;
                OnLoginSuccessful();
            }
            else
            {
                MyLogin.sessions.status_message = response.StatusCode.ToString();
                OnLoginFailed();
            }
            //return myLogin;
        }

        /// <summary>
        /// Retrieve Location Data Asyncroniously
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Models.Locations> GetGardenaLocationAsync(string user_id, string token)
        {
            Models.Locations myLocations = null;
            HttpClient myHttpClient = new HttpClient();
            string url = urls.GetLocationsUrl + user_id;

            myHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            myHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Session", token);

            var response = await myHttpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                myLocations = JsonConvert.DeserializeObject<Models.Locations>(result);
                myLocations.status_message = response.StatusCode.ToString();

            }
            else
            {
                myLocations.status_message = response.StatusCode.ToString();
            }

            return myLocations;
        }

        /// <summary>
        /// Returns details over all Device Configurations
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Models.Devices> GetGardenaDevicesAsync(string locationId, string token)
        {
            Models.Devices myDevices = null;
            HttpClient myHttpClient = new HttpClient();
            string url = urls.GetDevicesUrl + locationId;

            myHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            myHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Session", token);

            var response = await myHttpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                myDevices = JsonConvert.DeserializeObject<Models.Devices>(result);
                myDevices.status_message = response.StatusCode.ToString();

            }
            else
            {
                myDevices.status_message = response.StatusCode.ToString();
            }
            return myDevices;
        }

        /// <summary>
        /// command: GardenaSendCommandAsync
        /// command: park_until_next_timer => Parken bis zum nächsten Zeitplan
        /// command: park_until_further_notice => Parken und Zeitplan pausieren
        /// command: start_resume_schedule => Parken und Zeitplan pausieren
        /// command: start_override_timer parameter: 1440 => 24 Stunden mähen
        /// command: start_override_timer parameter: 4320 => 3 Tage mähen
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="locationID"></param>
        /// <param name="token"></param>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<Models.SendCommand> GardenaSendCommandAsync(string deviceID, string locationID, string token, string command, int parameter = 0)
        {
            HttpClient myHttpClient = new HttpClient();
            myHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Session", token);
            Models.SendCommand myCommand = new Models.SendCommand();
            Models.Parameters myParam = new Models.Parameters();
            myParam.duration = parameter;
            myCommand.name = command;
            myCommand.parameters = myParam;

            var myRequestString = JsonConvert.SerializeObject(myCommand);
            var response = myHttpClient.PostAsync(urls.SendCommandUrl + deviceID + "/abilities/mower/command?locationId=" + locationID, new StringContent(myRequestString.ToString(), Encoding.UTF8, "application/json")).Result;

            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                //myCommand = JsonConvert.DeserializeObject<Models.SendCommand>(result);
                //myCommand.status_message = response.StatusCode.ToString();
            }
            else
            {
                //myCommand.status_message = response.StatusCode.ToString();
            }

            return myCommand;
        }

        #endregion

        #region Events
        protected virtual void OnLoginSuccessful()
        {
            LoginSuccessful?.Invoke(this);
        }
        protected virtual void OnLoginFailed()
        {
            LoginFailed?.Invoke(this);
        }
        #endregion
    }
}
