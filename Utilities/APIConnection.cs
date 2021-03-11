using CodingChallenge.AttributeModels;

namespace CodingChallenge.Utilities
{
    public class APIConnection : JsonSource
    {
        private RequestAttribute requestAttribute;
        public APIConnection(RequestAttribute requestAttribute)
        {
            this.requestAttribute = requestAttribute;
        }

        public string getJSON()
        {
            //This would be implemented if I was able to access an API key for the DealerRater API.
            throw new System.NotImplementedException();
        }
    }
}
