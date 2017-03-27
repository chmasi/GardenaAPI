# GardenaAPI
API to steer Gardena Sileno Mower

What needs to be done:
1. All async functions needs to be tested and completed.
2. Async function for schedule needs to be added.


For the first start:

using Gardena.NET.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

readonly GardenaAPI gardenaApi = new GardenaAPI();
Locations GardenaLocation = new Locations();
Devices GardenaDevices = new Devices();

private async void GardenaApi_LoginSuccessful(object sender)
{
  gardenaApi.LoginSuccessful += GardenaApi_LoginSuccessful;
  gardenaApi.LoginFailed += GardenaApi_LoginFailed;
  gardenaApi.GardenaLoginAsync(Properties.Settings.Default.GardenaUser, Properties.Settings.Default.GardenaUserPassword);
}

private void GardenaApi_LoginFailed(object sender)
{
  throw new NotImplementedException();
}

private async void GardenaApi_LoginSuccessful(object sender)
{
  var MowerLog = new List<MowerLog>();
  GardenaLocation = await gardenaApi.GetGardenaLocationAsync(((GardenaAPI)sender).MyLogin.sessions.user_id,         ((GardenaAPI)sender).MyLogin.sessions.token);
  GardenaDevices = await gardenaApi.GetGardenaDevicesAsync(myGardenaLocation.locations[0].id, ((GardenaAPI)sender).MyLogin.sessions.token);

  foreach (var item in myGardenaDevices.devices[1].status_report_history)
  {
     MowerLog.Add(JsonConvert.DeserializeObject<MowerLog>(item.ToString()));
  }

  Session.Add(name: "MowerLog", value: MowerLog);

  foreach (var item in myGardenaDevices.devices)
  {
                if (item.category.ToLower() == "gateway")
                {

                }
                if (item.category.ToLower() == "mower")
                {
                    mowerStatus.Clear();
                    MowerName = item.name;
                }

   }
}
