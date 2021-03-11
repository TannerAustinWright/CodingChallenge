﻿using CodingChallenge.AttributeModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CodingChallenge.Utilities
{
    class WebScraper : JsonSource
    {
        private RequestAttribute requestAttribute;

        public WebScraper(RequestAttribute requestAttribute)
        {
            this.requestAttribute = requestAttribute;
        }

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
                string endpoint;
                if (i == 0)
                {
                    endpoint = "";
                } else
                {
                    endpoint = $"/page{i + 1}";
                }
                // Request the page at the endpoint, read the text from the response as a string, and
                // append it to the HTML string to return.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestAttribute.URL + endpoint);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html += reader.ReadToEnd();
                }
            }

            return html;
        }

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
