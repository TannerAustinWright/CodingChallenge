using System.Collections.Generic;

namespace CodingChallenge.AttributeModels
{
    public class AnalyzerRule
    {
        public string Key { get; set; }
        public List<KeyWeight> Rules { get; set; }
    }

    public class KeyWeight
    {
        public string Keyword { get; set; }
        public int Weight { get; set; }
    }
}
