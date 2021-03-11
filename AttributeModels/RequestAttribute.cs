namespace CodingChallenge.AttributeModels
{
    public class RequestAttribute
    {
        public string URL;
        public bool API;
        public string AccessToken;
        public string DealerID;
        public PaginationOptions PaginationOptions;
        public string URI;
    }

    public class PaginationOptions
    {
        public int Pages;
        public int ResultsPerPage;
        public int Offset;
    }
}
