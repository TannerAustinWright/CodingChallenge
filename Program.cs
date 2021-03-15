using System;
using CodingChallenge.Utilities;
using CodingChallenge.AttributeModels;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using CodingChallenge.APIModels;
using System.Diagnostics;

namespace CodingChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> attrList = new List<string>(){ @"AnalyzerAttributes.json", @"RequestAttributes.json" };
            List<AnalyzerRule> analyzerAttrs;
            RequestAttribute requestAttr;
            JsonSource json;
            JsonAnalyzer<Review> jsonAnalyzer;

            //  Opend and read the JSON files containing the requset and JSON analyzer settings.
            using (StreamReader reader = new StreamReader(attrList[0]))
            {
                analyzerAttrs = JsonConvert.DeserializeObject<List<AnalyzerRule>>(reader.ReadToEnd());
            }
            using (StreamReader reader = new StreamReader(attrList[1]))
            {
                requestAttr = JsonConvert.DeserializeObject<RequestAttribute>(reader.ReadToEnd());
            }

            //  Determine whether or not we can use the api to get the reviews. (APIConnection could
            //  not be implemented because I was unable to obtain an access token.
            if (!requestAttr.API)
            {
                json = new WebScraper(requestAttr);
            }
            else
            {
                //json = new APIConnection(requestAttr);
                Console.WriteLine("Error: Non-WebScraper method not suppoerted.");
                Console.ReadKey();
                return;
            }

            //  Initialize the analyzer with the desired analysys rules.
            jsonAnalyzer = new JsonAnalyzer<Review>(analyzerAttrs);

            //  Extract the list of reviews from the json string by deserializing the object.
            var stopwatch = Stopwatch.StartNew();
            List<Review> reviewsAsync = JsonConvert.DeserializeObject<ReviewCollection>(json.getJSONAsync().Result).reviews;
            stopwatch.Stop();
            Console.WriteLine($"Async Runtime: {stopwatch.Elapsed}");

            stopwatch.Restart();
            List<Review> reviewsSync = JsonConvert.DeserializeObject<ReviewCollection>(json.getJSON()).reviews;
            stopwatch.Stop();
            Console.WriteLine($"Sync Runtime: {stopwatch.Elapsed}");

            //  Run the analyzer passing the reviews as a JSON object string imitating the format of 
            //  the API response (in case future implementation is possible).
            jsonAnalyzer.runRules(JsonConvert.SerializeObject(reviewsAsync));

            //  Get the results from the analyzer object and output them to the console.
            foreach(KeyValuePair<Review, int> kvp in jsonAnalyzer.getTop(3))
            {
                Console.WriteLine($"Positivity Score: {kvp.Value}\nReview:");
                Console.WriteLine($"{kvp.Key.comments}\n");
            }

            // Wait.
            Console.ReadKey();
        }
    }
}
