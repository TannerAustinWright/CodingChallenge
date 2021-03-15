using System.Threading.Tasks;

namespace CodingChallenge.Utilities
{
    public interface JsonSource
    {
        public string getJSON();
        public Task<string> getJSONAsync();
    }
}
