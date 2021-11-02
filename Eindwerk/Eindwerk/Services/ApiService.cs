using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Eindwerk.Services
{
    public class ApiService
    {
        /**
         * <value>the JWT Access Token to use in requests</value>
         */
        protected string AccessToken;

        /**
         * <value>The base uri for calling the api</value>
         */
        protected const string BASEURI = "http://localhost:7071/api";

        protected ApiService(string accessToken)
        {
            AccessToken = accessToken;
        }

        /**
         * <summary>Performs any request that needs provided JSON <paramref name="payload"/> and returns as <typeparamref name="TReturn"/></summary>
         *
         * <param name="doRequest">a function that runs the actual request given the <c>HttpContent</c> to send</param>
         * <param name="payload">the payload to send</param>
         * <param name="debugCall">whether additional debug information should be included or not</param>
         *
         * <typeparam name="TReturn">the return type</typeparam>
         *
         * <returns>the request response as <typeparamref name="TReturn"/></returns>
         */
        private static async Task<TReturn> DoAnyPayloadRequest<TReturn>(
            Func<HttpContent, Task<HttpResponseMessage>> doRequest,
            string payload,
            bool debugCall)
        {
            try
            {
                HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

                if (debugCall)
                {
                    Debug.WriteLine("payload to send: ");
                    Debug.WriteLine(payload);
                }

                HttpResponseMessage response = await doRequest(httpContent);

                if (debugCall)
                {
                    Debug.WriteLine("response:");
                    Debug.WriteLine(response);
                }

                if (response == null)
                {
                    return default;
                }

                var json = await response.Content.ReadAsStringAsync();

                if (debugCall)
                {
                    Debug.WriteLine("response content: ");
                    Debug.WriteLine(json);
                }

                return json == null ? default : JsonConvert.DeserializeObject<TReturn>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /**
         * <summary>Performs a GET request to the specified <paramref name="url"/> and returns the result as <typeparamref name="TReturn"/></summary>
         *
         * <param name="url">the url to request</param>
         * <param name="debugCall">if the request should be debugged (a little more logging info)</param>
         *
         * <typeparam name="TReturn">The return type of this function</typeparam>
         *
         * <returns>the result of the GET request as <typeparamref name="TReturn"/></returns>
         */
        protected async Task<TReturn> DoGetRequest<TReturn>(string url, bool debugCall = true)
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    var json = await client.GetStringAsync(url);

                    if (debugCall)
                    {
                        Debug.WriteLine("API Call response content: ");
                        Debug.WriteLine(json);
                    }

                    if (json == null)
                    {
                        Console.WriteLine("[WARNING] RESPONSE WAS NULL");
                        return default;
                    }

                    return JsonConvert.DeserializeObject<TReturn>(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        /**
         * <summary>performs POST request with specified <paramref name="payload"/> and returns result as <typeparamref name="TReturn"/></summary>
         *
         * <param name="url">the url to request</param>
         * <param name="payload">the payload to send</param>
         * <param name="debugCall">if the request should be debugged (a little more logging info)</param>
         *
         * <typeparam name="TReturn">The return type of this function</typeparam>
         *
         * <returns>the result of the POST request as <typeparamref name="TReturn"/></returns>
         */
        protected async Task<TReturn> DoPostRequest<TReturn>(string url, object payload, bool debugCall = true)
        {
            using (HttpClient client = GetClient())
            {
                return await DoAnyPayloadRequest<TReturn>(content => client.PostAsync(url, content),
                    JsonConvert.SerializeObject(payload),
                    debugCall);
            }
        }

        /**
         * <summary>performs PUT request with specified <paramref name="payload"/> and returns result as <typeparamref name="TReturn"/></summary>
         *
         * <param name="url">the url to request</param>
         * <param name="payload">the payload to send</param>
         * <param name="debugCall">if the request should be debugged (a little more logging info)</param>
         *
         * <typeparam name="TReturn">The return type of this function</typeparam>
         *
         * <returns>the result of the PUT request as <typeparamref name="TReturn"/></returns>
         */
        protected async Task<TReturn> DoPutRequest<TReturn>(string url, object payload, bool debugCall = true)
        {
            using (HttpClient client = GetClient())
            {
                return await DoAnyPayloadRequest<TReturn>(content => client.PutAsync(url, content),
                    JsonConvert.SerializeObject(payload), debugCall);
            }
        }

        private HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            if (AccessToken != null)
                client.DefaultRequestHeaders.Add("Authorization", AccessToken);
            return client;
        }
    }
}