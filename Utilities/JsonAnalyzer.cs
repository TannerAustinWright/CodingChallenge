using CodingChallenge.APIModels;
using CodingChallenge.AttributeModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodingChallenge.Utilities
{
    public class JsonAnalyzer
    {
        //identifies the top three most “overly positive” endorsements
        //(using criteria of your choosing, documented in the README)
        public ReviewCollection reviewCollection;
        private List<AnalyzerRule> analyzerRules;
        private Dictionary<Review, int> scores;
        public JsonAnalyzer(List<AnalyzerRule> analyzerRules)
        {
            this.analyzerRules = analyzerRules;
            reviewCollection = new ReviewCollection();
            scores = new Dictionary<Review, int>();
        }
        public void outputWordCount(string jsonString)
        {
            reviewCollection = JsonConvert.DeserializeObject<ReviewCollection>(jsonString);

            Dictionary<string,int> counts = new Dictionary<string,int>();
            foreach(Review r in reviewCollection.reviews)
            {
                Regex splitter = new Regex(@"[a-zA-Z]+");
                foreach (Match word in splitter.Matches(r.comments))
                {
                    string lowerWord = word.Value.ToLower();
                    if (counts.ContainsKey(lowerWord))
                    {
                        int count;
                        counts.TryGetValue(lowerWord, out count);
                        counts.Remove(lowerWord);
                        counts.Add(lowerWord, ++count);
                    }
                    else
                    {
                        counts.Add(lowerWord, 1);
                    }
                }
            }
            var orderedCounts = counts.OrderByDescending(p => p.Value);
            foreach(KeyValuePair<string,int> pair in orderedCounts)
            {
                Console.WriteLine($"{pair.Key} {pair.Value}");
            }
        }
        public void run(string jsonString)
        {
            reviewCollection = JsonConvert.DeserializeObject<ReviewCollection>(jsonString);

            foreach(Review review in reviewCollection.reviews)
            {
                int totalScore = 0;
                foreach(AnalyzerRule rule in analyzerRules)
                {
                    totalScore += score(review, rule);
                }
                scores.Add(review, totalScore);
            }
        }
        public List<KeyValuePair<Review, int>> getTop(int results)
        {
            return scores.OrderBy(kvp => kvp.Value).ToList();
        }
        private int score(Review review, AnalyzerRule rule)
        {
            int score = 0;
            Regex splitter = new Regex(@"[a-zA-Z]+");
            var propertyName = rule.Key;
            var propertyInfo = review.GetType().GetProperty(propertyName);
            var propertyContents = propertyInfo.GetValue(review, null);
            foreach (Match word in splitter.Matches(propertyContents.ToString())) //need to get comments from rule key
            {
                string lowerWord = word.Value.ToLower();
                
                if (rule.Rules.Where(r => r.Keyword == lowerWord).Count() != 0)
                {
                    score += rule.Rules.Where(r => r.Keyword == lowerWord).First().Weight;
                }
            }
            return score;
        }
    }
}
