using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Eindwerk.Repository
{
    public class ScraperRepository : ApiRepository
    {
        protected ScraperRepository(string baseuri) : base(baseuri) { }

        protected async Task<IHtmlDocument> ScrapePageAsync(string url)
        {
            using (HttpClient client = GetClient())
            {
                HttpResponseMessage request = await client.GetAsync($"{BASEURI}/{url}");

                Stream response = await request.Content.ReadAsStreamAsync();

                var parser = new HtmlParser();
                return await parser.ParseDocumentAsync(response);
            }
        }
    }
}