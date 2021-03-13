using CodingChallenge.APIModels;
using CodingChallenge.AttributeModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodingChallenge.Utilities
{
    public class JsonAnalyzer<t>
    {
        public List<t> collection;
        private List<AnalyzerRule> analyzerRules;
        private Dictionary<t, int> scores;
        public JsonAnalyzer(List<AnalyzerRule> analyzerRules)
        {
            this.analyzerRules = analyzerRules;
            collection = new List<t>();
            scores = new Dictionary<t, int>();
        }

        /// <summary>
        /// Outputs to the console the number of of times a specific word appears in a Collection 
        /// of reviews. I used this to help me identify positive terms that appeared in reveiws.
        /// </summary>
        /// <param name="jsonString"> The JSON representation of a collection of reviews as a string. </param>
        public void outputWordCount(string jsonString)
        {
            //  Convert the JSON string into objects.
            collection = JsonConvert.DeserializeObject<List<t>>(jsonString);

            //  Iterate through each review
            Dictionary<string,int> counts = new Dictionary<string,int>();
            foreach(t item in collection)
            {
                List<string> propertiesToInclude = new List<string>();
                analyzerRules.ForEach(rule => {
                    propertiesToInclude.Add(rule.Key);
                });

                propertiesToInclude.ForEach(propertyName =>
                {
                    //  Use reflection and the provided rule to determine the property to score.
                    var propertyInfo = item.GetType().GetProperty(propertyName);
                    var propertyContents = propertyInfo.GetValue(item, null);

                    //  Iterate through each word in a single review
                    Regex splitter = new Regex(@"[a-zA-Z]+");
                    foreach (Match word in splitter.Matches(propertyContents.ToString()))
                    {
                        //  Make all words lowercase for comparison
                        string lowerWord = word.Value.ToLower();
                        if (counts.ContainsKey(lowerWord))
                        {
                            //  If the word already exists in the count add one to the number of occurences.
                            int count;
                            counts.TryGetValue(lowerWord, out count);
                            counts.Remove(lowerWord);
                            counts.Add(lowerWord, ++count);
                        }
                        else
                        {
                            //  If the word does not already exist in the count dictonairy, add it.
                            counts.Add(lowerWord, 1);
                        }
                    }
                });
            }
            
            //  Order the words by their counts and output them to the console.
            var orderedCounts = counts.OrderByDescending(p => p.Value);
            foreach(KeyValuePair<string,int> pair in orderedCounts)
            {
                Console.WriteLine($"{pair.Key} {pair.Value}");
            }
        }

        /// <summary>
        /// Analyzes a JSON representation of reviews, and assigns each review a score based on
        /// keywords and their corresponding weight. Weight is determined by sentiment. Positive 
        /// sentiment results in a higher score, and negative will result in a lower, possibly negative
        /// score. Results are stored in the Review Collection.
        /// </summary>
        /// <param name="jsonString"></param>
        public void runRules(string jsonString)
        {
            //  Convert the JSON string into objects.
            collection = JsonConvert.DeserializeObject<List<t>>(jsonString);

            //  For each ojbect, run each included rule on the review and assign it a score.
            foreach(t item in collection)
            {
                int totalScore = 0;
                foreach(AnalyzerRule rule in analyzerRules)
                {
                    try
                    {
                        totalScore += score(item, rule);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                scores.Add(item, totalScore);
            }
        }

        /// <summary>
        /// Returns a list of key value pairs. Keys are reviews, and values are their corresponding 
        /// Positivity score. This list will only contain results after the run() function is called.
        /// </summary>
        /// <param name="results"> The number of records to return. If an integer value of 0 is passed or no parameters
        /// all results will be returned instead of an empty list. </param>
        /// <returns> A list of Keyvalue pairs of type <Review, int> </returns>
        public List<KeyValuePair<t, int>> getTop(int results = 0)
        {
            if (results == 0) results = scores.Count;
            return scores.OrderByDescending(kvp => kvp.Value).Take(results).ToList();
        }

        /// <summary>
        /// Runs a specific analyzer rule on a review record and returns the a score based on the words 
        /// weight. Attributes, weights, and keywords are defined using the analyzerRule type.
        /// </summary>
        /// <param name="review"> The review object to score. </param>
        /// <param name="rule"> The rule to run on a particurlar rule object. </param> 
        /// <returns> The score based on a specific rule set for the review parameter. </returns>
        private int score(t item, AnalyzerRule rule)
        {
            //  RAII
            int score = 0;

            //  REGEX to capture only word characters
            Regex splitter = new Regex(@"[a-zA-Z]+");

            //  Use reflection and the provided rule to determine the property to score.
            var propertyName = rule.Key;
            var propertyInfo = item.GetType().GetProperty(propertyName);
            object propertyContents;
            try
            {
                propertyContents = propertyInfo.GetValue(item, null);
            } 
            catch (Exception e)
            {
                throw e;
            }

            //  Iterate each word, and if included add the weight to the total score. (may be negative)
            foreach (Match word in splitter.Matches(propertyContents.ToString()))
            {
                string lowerWord = word.Value.ToLower();
                
                if (rule.Rules.Where(r => r.Keyword.ToLower() == lowerWord).Count() != 0)
                {
                    score += rule.Rules.Where(r => r.Keyword.ToLower() == lowerWord).First().Weight;
                }
            }
            return score; //return the score.
        }
    }
}
