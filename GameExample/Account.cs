using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace GameExample
{
    public static class Account
    {
        public static async Task<string> RegisterDomainAsync(string ip, string url)
        {
            string result = "";

            try
            {

                var request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { url, ip},
                    }
                };

                var update = PlayFabClientAPI.UpdateUserDataAsync(request);

                await update.ContinueWith((x) =>
                {
                    if (update.Result.Error != null)
                    {
                        result += update.Result.Error.ErrorMessage;
                    }
                    else if (update.Result.Result != null)
                    {
                        result += $"{url} is transmitting over the cloud.";
                    }
                });

            }
            catch (Exception e)
            {
                return e.Message;
            }
            return result;
        }

        public static async Task<string> GetIPAsync(string url)
        {
            var ip = "";

            try
            {


                var request = new GetUserDataRequest
                {
                    Keys = new List<string>() { url },
                };

                var data = PlayFabClientAPI.GetUserDataAsync(request);

                await data.ContinueWith((x) =>
                {
                    if (data.Result.Error != null)
                    {
                        ip = "ERROR -- " + data.Result.Error.ErrorMessage;
                    }
                    else if (data.Result.Result != null)
                    {
                        ip = data.Result.Result.Data[url].Value;
                    }
                });


            }
            catch (Exception e)
            {
                ip = "ERROR -- " + e.Message;
            }

            return ip;
        }

        public static async Task<string> LoginAsync()
        {
            string result = "";
            try
            {

                var request = new LoginWithCustomIDRequest
                {
                    CustomId = "Universal-ID",
                    CreateAccount = true
                };

                var login = PlayFabClientAPI.LoginWithCustomIDAsync(request);

                await login.ContinueWith((x) =>
                {
                    if (login.Result.Error != null)
                    {
                        result += login.Result.Error.ErrorMessage;
                    }
                    else if (login.Result.Result != null)
                    {
                        result += $"Has connected to cloud.";
                    }
                });
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return result;

        }
    }
}
