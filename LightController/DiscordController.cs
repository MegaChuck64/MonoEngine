using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LightController
{
    public delegate void LogHandler(string msg);


    public static class DiscordController
    {
        public static HttpClient client;

        static LogHandler Log;

        public static void InitController(LogHandler logHandler)
        {
            Log = logHandler;

            try
            {
                Task mainTask = new Task(RunAsync);
                mainTask.Start();
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        }

        static string baseUrl = @"https://discordapp.com/api";
        static int timeout = 90;

        static async void RunAsync()
        {        
            client = new HttpClient();

            client.BaseAddress = new Uri(baseUrl);
            
            client.Timeout = new TimeSpan(0, 0, timeout);

            string _ContentType = "application/json";

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ContentType));

            var _CredentialBase64 = GetBotToken();

            client.DefaultRequestHeaders.Add("Authorization", String.Format("Basic {0}", _CredentialBase64));

            var _UserAgent = "cj HttpClient";
            // You can actually also set the User-Agent via a built-in property
            client.DefaultRequestHeaders.Add("User-Agent", _UserAgent);
            // You get the following exception when trying to set the "Content-Type" header like this:
            // client.DefaultRequestHeaders.Add("Content-Type", _ContentType);
            // "Misused header name. Make sure request headers are used with HttpRequestMessage, response headers with HttpResponseMessage, and content headers with HttpContent objects."

            HttpResponseMessage response;

            var Method = "HEAD";

            switch (Method.ToString().ToUpper())
            {
                case "GET":
                case "HEAD":
                    // synchronous request without the need for .ContinueWith() or await
                    response = await client.GetAsync(baseUrl);
                    break;
                //case "POST":
                //    {
                //        // Construct an HttpContent from a StringContent
                //        HttpContent _Body = new StringContent(Body);
                //        // and add the header to this object instance
                //        // optional: add a formatter option to it as well
                //        _Body.Headers.ContentType = new MediaTypeHeaderValue(_ContentType);
                //        // synchronous request without the need for .ContinueWith() or await
                //        response = client.PostAsync(baseUrl, _Body).Result;
                //    }
                //    break;
                //case "PUT":
                //    {
                //        // Construct an HttpContent from a StringContent
                //        HttpContent _Body = new StringContent(Body);
                //        // and add the header to this object instance
                //        // optional: add a formatter option to it as well
                //        _Body.Headers.ContentType = new MediaTypeHeaderValue(_ContentType);
                //        // synchronous request without the need for .ContinueWith() or await
                //        response = client.PutAsync(baseUrl, _Body).Result;
                //    }
                //    break;
                //case "DELETE":
                //    response = client.DeleteAsync(baseUrl).Result;
                //    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
            // either this - or check the status to retrieve more information
            response.EnsureSuccessStatusCode();
            // get the rest/content of the response in a synchronous way
            var resMsg = await response.Content.ReadAsStringAsync();

            Log(resMsg);

        }


        private static string GetBotToken()
        {
            string tkn = Environment.GetEnvironmentVariable("dscrdBtTkn", EnvironmentVariableTarget.User);
            return tkn;
        }

    }
}

//public static void SetHeader(string key, string value)
//{
//    client.DefaultRequestHeaders.Remove(key);
//    if (value != null)
//        client.DefaultRequestHeaders.Add(key, value);
//}

//static async void RunAsync()
//{
//    client = new HttpClient();

//    string tkn = GetBotToken();

//    SetHeader("accept", "*/*");
//    SetHeader("authorization", $"Bot {tkn}");

//    byte[] tknBytes = Encoding.ASCII.GetBytes(tkn);


//    HttpResponseMessage response = await client.GetAsync("");


//    HttpContent content = response.Content;

//    // ... Check Status Code                                
//    Log($"Response StatusCode: {(int)response.StatusCode}");

//    // ... Read the string.
//    string result = await content.ReadAsStringAsync();

//    Log(result);
//}
