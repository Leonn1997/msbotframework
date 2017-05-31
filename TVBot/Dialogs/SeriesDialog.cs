using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HttpUtils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Framework.Builder.Witai;
using Microsoft.Bot.Framework.Builder.Witai.Dialogs;
using Microsoft.Bot.Framework.Builder.Witai.Models;
using Newtonsoft.Json;
using TVBot.Models;

namespace TVBot.Dialogs
{
    [Serializable]
    [WitModel("7JJ6P6X2DRA7IMAZMYZATPNTI334BGPO")]
    public class SeriesDialog : WitDialog<object>
    {
        [WitAction("getRecommendations")]
        public async Task GetRecommendations(IDialogContext context, WitResult result)
        {
            var series = result.Entities["search_query"]
                .Select(a => a.Value)
                .Select(a => a.ToString() )
                .ToList();

            var r = await FindSimilarShows(series);

            if (r != null && r.Count > 0)
            {
                //this.WitContext.RemoveIfExists("no_result");
                //this.WitContext["results"] = r;
                this.WitContext.RemoveIfExists("no_results");
                this.WitContext.AddOrUpdate("results", r);
            } else
            {
                this.WitContext.RemoveIfExists("results");
                this.WitContext.AddOrUpdate("no_result", null);

            }

        }

        public async Task<List<String>> FindSimilarShows(List<String> shows)
        {
            var showParm = "similar?q=" + ExtractShowParms(shows) + "&type=shows&info=1&k=155111-EPort-ZKC22N7Q";
            string endPoint = @"https://tastedive.com/api/";


            var client = new RestClient(endPoint);
            client.Method = HttpVerb.GET;
            var json = client.MakeRequest(showParm);

            var root = JsonConvert.DeserializeObject<RootObject>(json);

            return root.Similar.Results.Where(a => a != null).Select(r => r.Name).ToList();
        }

        private static String ExtractShowParms(List<string> shows)
        {
            const string seperator = "%2C";
            const string space = "+";
            var builder = new StringBuilder();
            foreach (var value in shows)
            {
                builder.Append(value.Replace(" ", space));
                builder.Append(seperator);
            }

            return builder.ToString();
        }
    }
}