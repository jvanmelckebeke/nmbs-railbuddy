using System.Net.Http;

namespace Eindwerk.Repository
{
    public class ApiRepository
    {
        /// <summary>
        ///     the base uri to call for the api
        /// </summary>
        protected string BASEURI;

        protected ApiRepository(string baseuri)
        {
            BASEURI = baseuri;
        }

        protected virtual HttpClient GetClient()
        {
            return new HttpClient();
        }
    }
}