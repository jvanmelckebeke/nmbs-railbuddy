using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Eindwerk.Exceptions;
using Eindwerk.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eindwerk.Repository
{
    public class ApiRepository
    {
        /// <summary>
        /// the base uri to call for the api
        /// </summary>
        protected string BASEURI;

        protected ApiRepository(string baseuri)
        {
            BASEURI = baseuri;
        }

        /// <summary>
        /// Performs any request that needs provided JSON <paramref name="payload"/> and returns as <typeparamref name="TReturn"/>
        /// </summary>
        ///
        /// <param name="doRequest">a function that runs the actual request given the <c>HttpContent</c> to send</param>
        /// <param name="payload">the payload to send</param>
        /// <param name="debugCall">whether additional debug information should be included or not</param>
        ///
        /// <typeparam name="TReturn">the return type</typeparam>
        ///
        /// <returns>the request response as <typeparamref name="TReturn"/></returns>
        private static async Task<TReturn> DoAnyPayloadRequest<TReturn>(
            Func<HttpContent, Task<HttpResponseMessage>> doRequest,
            string payload,
            bool debugCall) where TReturn : IDtoModel
        {
            HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

            if (debugCall) Debug.WriteLine($"payload to send: {ConvertToFormattedJson(payload)}");

            HttpResponseMessage response;

            try
            {
                response = await doRequest(httpContent);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] NetworkException");
                Debug.WriteLine(e);
                throw new NoNetworkException();
            }


            return await FinalizeRequestAsync<TReturn>(response, debugCall);
        }

        /// <summary>
        /// Performs a GET request to the specified <paramref name="url"/> and returns the result as <typeparamref name="TReturn"/>
        /// </summary>
        ///
        /// <param name="url">the url to request</param>
        /// <param name="debugCall">if the request should be debugged (a little more logging info)</param>
        ///
        /// <typeparam name="TReturn">The return type of this function</typeparam>
        ///
        /// <returns>the result of the GET request as <typeparamref name="TReturn"/></returns>
        protected async Task<TReturn> DoGetRequest<TReturn>(string url, bool debugCall = true) where TReturn : IDtoModel
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    return await FinalizeRequestAsync<TReturn>(response, debugCall);
                }
                catch (HttpRequestException e)
                {
                    Debug.WriteLine($"[ERROR] NetworkException for url {url}");
                    Debug.WriteLine(e);
                    throw new NoNetworkException();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        /// <summary>
        /// performs POST request with specified <paramref name="payload"/> and returns result as <typeparamref name="TReturn"/>
        /// </summary>
        ///
        /// <param name="url">the url to request</param>
        /// <param name="payload">the payload to send</param>
        /// <param name="debugCall">if the request should be debugged (a little more logging info)</param>
        ///
        /// <typeparam name="TReturn">The return type of this function</typeparam>
        ///
        /// <returns>the result of the POST request as <typeparamref name="TReturn"/></returns>
        protected async Task<TReturn> DoPostRequest<TReturn>(string url, object payload, bool debugCall = true)
            where TReturn : IDtoModel
        {
            using (HttpClient client = GetClient())
            {
                return await DoAnyPayloadRequest<TReturn>(content => client.PostAsync(url, content),
                    JsonConvert.SerializeObject(payload),
                    debugCall);
            }
        }

        /// <summary>
        /// performs PUT request with specified <paramref name="payload"/> and returns result as <typeparamref name="TReturn"/>
        /// </summary>
        ///
        /// <param name="url">the url to request</param>
        /// <param name="payload">the payload to send</param>
        /// <param name="debugCall">if the request should be debugged (a little more logging info)</param>
        ///
        /// <typeparam name="TReturn">The return type of this function</typeparam>
        ///
        /// <returns>the result of the PUT request as <typeparamref name="TReturn"/></returns>
        protected async Task<TReturn> DoPutRequest<TReturn>(string url, object payload, bool debugCall = true)
            where TReturn : IDtoModel
        {
            using (HttpClient client = GetClient())
            {
                return await DoAnyPayloadRequest<TReturn>(content => client.PutAsync(url, content),
                    JsonConvert.SerializeObject(payload), debugCall);
            }
        }

        /// <summary>
        /// handles the return value of a given http <paramref name="response"/>, including debug logging
        /// </summary>
        /// <param name="response">the http response of the request</param>
        /// <param name="debugCall">whether to log additional information or not</param>
        /// <typeparam name="TReturn">The type to return, *must* implement <see cref="IDtoModel"/></typeparam>
        /// <returns>If the request was a success, an object of type <typeparamref name="TReturn"/>, otherwise <c>null</c></returns>
        private static async Task<TReturn> FinalizeRequestAsync<TReturn>(HttpResponseMessage response,
            bool debugCall = true)
            where TReturn : IDtoModel
        {
            if (debugCall) Debug.WriteLine($"response code: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response);
                throw new Exception("something went wrong with the API");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (debugCall) Debug.WriteLine($"response content: {ConvertToFormattedJson(jsonResponse)}");

            if (jsonResponse == null)
            {
                if (debugCall) Debug.WriteLine($"WARNING: response content is null");

                return default;
            }

            TReturn valueToReturn = JsonConvert.DeserializeObject<TReturn>(jsonResponse);

            if (debugCall) Debug.WriteLine($"translated response object: {valueToReturn}");

            if (valueToReturn != null && valueToReturn.IsFilled()) return valueToReturn;


            if (debugCall) Debug.WriteLine("WARNING: response object is null or invalid");
            return default;
        }

        /// <summary>
        /// creates a default HTTPClient, can be overridden to include for example Authorization headers
        /// </summary>
        /// 
        /// <returns>a HTTP client</returns>
        protected virtual HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }


        private static string ConvertToFormattedJson(string json)
        {
            JToken jt = JToken.Parse(json);
            return jt.ToString(Formatting.Indented);
        }
    }
}