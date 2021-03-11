using System;
using System.Collections.Generic;
using System.Text;

namespace CodingChallenge.APIModels
{
    public class ReviewCollection
    {
        public string dealerId { get; set; }
        public string ratingURl { get; set; }
        public string name { get; set; }
        public int reviewCount { get; set; }
        public List<Review> reviews { get; set; }
    }

    public class Review
    {
        public string id { get; set; }
        public string dateWritten { get; set; }
        public string title { get; set; }
        public string comments { get; set; }
        public string recommendDealer { get; set; }
        public int score { get; set; }
    }
}
