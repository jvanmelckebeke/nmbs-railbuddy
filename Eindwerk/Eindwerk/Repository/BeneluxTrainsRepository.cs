using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Eindwerk.Models.Rail;

namespace Eindwerk.Repository
{
    public class BeneluxTrainsRepository : ScraperRepository
    {
        public BeneluxTrainsRepository() : base("https://www.beluxtrains.net") { }

        public async Task<List<Wagon>> GetTrainCompositionAsync(string vehicleNumber)
        {
            IHtmlDocument document =
                await ScrapePageAsync($"indexen.php?page=belgium-files/2021_2/2021-{vehicleNumber}");

            IElement firstComposition = document.QuerySelector("table#composition");

            var wagons = firstComposition.QuerySelectorAll("tr")
                                   .Select(tableRow =>
                                       new Wagon()
                                       {
                                           ModelName = tableRow.QuerySelector(".cell3").TextContent,
                                           ModelUrl = tableRow.QuerySelector("img").Attributes["src"].Value
                                       }).ToList();

            for (var index = 0; index < wagons.Count; index++)
            {
                Wagon wagon = wagons[index];
                Debug.WriteLine($"Wagon {index} : {wagon}");
            }

            return wagons;
        }
    }
}