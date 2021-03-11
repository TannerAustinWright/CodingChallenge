using System;
using CodingChallenge.Utilities;
using CodingChallenge.AttributeModels;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using CodingChallenge.APIModels;

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
            JsonAnalyzer jsonAnalyzer;

            using (StreamReader reader = new StreamReader(attrList[0]))
            {
                analyzerAttrs = JsonConvert.DeserializeObject<List<AnalyzerRule>>(reader.ReadToEnd());
            }
            using (StreamReader reader = new StreamReader(attrList[1]))
            {
                requestAttr = JsonConvert.DeserializeObject<RequestAttribute>(reader.ReadToEnd());
            }

            if (!requestAttr.API)
            {
                json = new WebScraper(requestAttr);
            }
            else
            {
                json = new APIConnection(requestAttr);
            }

            jsonAnalyzer = new JsonAnalyzer(analyzerAttrs);

            jsonAnalyzer.run(json.getJSON());

            foreach(KeyValuePair<Review, int> kvp in jsonAnalyzer.getTop(50))
            {
                Console.WriteLine($"Positivity Score: {kvp.Value}\nReview:");
                Console.WriteLine($"{kvp.Key.comments}\n");
            }
            Console.ReadKey();
        }
    }
}
