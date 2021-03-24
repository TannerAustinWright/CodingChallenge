using CodingChallenge.AttributeModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CodingChallenge.Utilities
{
    public class WebScraper : JsonSource
    {
        private RequestAttribute requestAttribute;

        public WebScraper(RequestAttribute requestAttribute)
        {
            this.requestAttribute = requestAttribute;
        }

        /// <summary>
        /// Sends an Http request to get pages of reviews for a specific URL, and number of
        /// subsequent pages. This method is specifically for review endpoints of DealerRater
        /// web urls.
        /// </summary>
        /// <returns> The HTML content of all pages concatenated into a single string. </returns>
        public string getHTML()
        {
            string html = "";

            // For each page to pull reviews from you will need a different url.
            // Loop as many pages as designated in the pagination options.
            for (int i = requestAttribute.PaginationOptions.Offset; 
                i < requestAttribute.PaginationOptions.Pages + requestAttribute.PaginationOptions.Offset; 
                i++)
            {
                // If the index of the page is zero the page has no additional endpoint appended. 
                // If the index is 1 or greater append the correct endpoint to the URL with a 1 based index.
                string endpoint = "";
                if (i != 0)
                {
                    if (!requestAttribute.URL.Contains("www.dealerrater.com")) throw new Exception("Scraping multiple pages is only supported for DealerRater URL's");
                    endpoint = $"/page{i + 1}";
                }
                // Request the page at the endpoint, read the text from the response as a string, and
                // append it to the HTML string to return.
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestAttribute.URL + endpoint);
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        html += reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return html;
        }

        /// <summary>
        /// Calls the getHTML() function retreiving the HTML string of desired webpages, extracts all
        /// <p> tags wich contain the class 'review-content', and formats the content of the tag as a JSON
        /// object that mimics the representation of the DealerRater API.
        /// </summary>
        /// <returns> The review data from the desired pages in a JSON formatted string. </returns>
        public string getJSON()
        {
            // Use getHtml() to requset the text of all pages in a single string.
            string html = getHTML();

            // Use regex to find the review bodies.
            Regex regex = new Regex("<p class=\".*review-content.*\".*>([^<]*?)<\\/p>");
            /*
            Regex: 
            <p class=\".*review-content.*\".*>([^<]*?)<\\/p>
            <p class=                   String literal. The match must be a p tag with a class attribute.
            \".*review-content.*\"      The class attribute must contain the string "review-content" and can have other classes,
                                        hence the leading and trailing .*
            .*>                         The tag may contain other attributes before the content.
            ([^<]*?)                    The contents of the tag may be any character other than a '<' if you use a '.' the regex will 
                                        Not match comments that span multiple lines.
            <\\/p>                      Must also end in a </p> tag.
            */
            var matches = regex.Matches(html);

            // Store the text of each review in a list of objects to mimic the structure of the API's JSON.
            List<object> reviews = new List<object>();
            foreach (Match match in matches)
            {
                reviews.Add(new{ comments = match.Groups[1].ToString() });
            }

            // Return a JSON representation of the object containing the list of reviews.
            return JsonConvert.SerializeObject(new { reviews = reviews });
        }
    }
}
