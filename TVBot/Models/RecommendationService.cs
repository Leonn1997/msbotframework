using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttpUtils;
using TVBot.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

/// <summary>
/// Summary description for Class1
/// </summary>
public class RecommendationService
{
	public RecommendationService()
	{
        
    }
    public async Task<String[]> FindSimilarShows(List<String> shows)
    {
        var showParm = "similar?q=" + ExtractShowParms(shows) + "&type=shows&info=1&k=155111-EPort-ZKC22N7Q";
        string endPoint = @"https://tastedive.com/api/";


        var client = new RestClient(endPoint);
        client.Method = HttpVerb.GET;
        var json = client.MakeRequest(showParm);

        var root = JsonConvert.DeserializeObject<RootObject>(json);

        return root.Similar.Results.Where(a => a != null).Select(r => r.Name).ToArray();
    }

    public async Task<String> PrittyResult(List<String> shows)
    {
        var l = await FindSimilarShows(shows);
        var builder = new StringBuilder();
        for(int i = 0; i < l.Count(); i++)
        {
            if(i == l.Count() -1 )
            {
                builder.Append(" and ");
                builder.Append(l[i]);
            } else if(i == l.Count() - 2)
            {
                builder.Append(l[i]);
            } else {
                builder.Append(l[i]);
                builder.Append(", ");
            }
           
        }

        return builder.ToString();
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
