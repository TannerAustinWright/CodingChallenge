		
		Podium Coding Challenge
		Tanner Wright

		-- Goal:
		Coding Challenge: “A Dealer For the People”
		The KGB has noticed a resurgence of overly excited reviews for a McKaig Chevrolet Buick, a dealership they have planted in the United States. 
		In order to avoid attracting unwanted attention, you’ve been enlisted to scrape reviews for this dealership from DealerRater.com and uncover 
		the top three worst offenders of these overly positive endorsements.

		Your mission, should you choose to accept it, is to write a tool that:

		1 scrapes the first five pages of reviews
		2 identifies the top three most “overly positive” endorsements (using criteria of your choosing, documented in the README)
		3 outputs these three reviews to the console, in order of severity
		Please include documentation on how to run your application along with how to run the test suite. 

		-- How it Works:
		The core functionalty has been built into two classes: WebScraper and JsonAnalyzer. Each of these classes depents on a json object passed to
		the constructor which initializes their settings. 
		
		WebScraper requires an object containing a URL to send the request to, and the number of 
		pages to pull from that URL. The object must also contain pagination options indicating how many pages to 'scrape' and an optional offset. 
		multiple pages are only supported for dealer rater sites because their api does not support HATEOS. For this reason I have to construct the
		endpoints manually using their convention of appending "/pageX" to the URL where X is the page number, and page number one has no appended 
		endpoint to the URL. This is why multi-pagination is only supported for dealerrater pages as other domains may paginate with a different 
		convention.

		After the request is made to dealerrater and the pages are pulled the HTML content is concatenated into a single string, and REGEX is used to
		find all reviews inside of <p> tags witht the correct classes. The reviews are then converted into a JSON forma to mimic the unstructured 
		data the the DealerRater API returns. I did this so that if I were to get an accessToken for the dealerRater API i could easly swap out an 
		api connection out with my web scraper which would be much more efficient. I was unable to get an access token so the web scraper is the 
		method I used.

		The json data is then fed into an anitialized JsonAnalyzer object. The constructor for the JsonAnalyzer takes a set of analyzer rules that 
		are used to score the key value pairs of the objects that the analyzer is Run() on. The JsonAnalyzer is a generic type, and be used on any
		list of objects to score their attributes. This would be easier in java, but I wanted to flex my c# reflection skills. (This is a joke. 
		Javascript would definitely handle this more neatly, but I thought it would be a fun to implement in C#). My unit tests contain a test 
		showing that you can use the JsonAnalyzer class on objects other than reviews. 

		The attributes I have intialized my JsonAnalyzer with can be found in the AnalyzerAttributes file in the Bin. I have included copies in the 
		project's ConfigurationJson folder so that you can find them quickly. The object contains a list of rules, and each rule contains a "key" which 
		corresponds to a key in the object to be analyzed. Each object also contains a list of KeyWeight objects which are keywords, and their 
		assigned weight. My document contains keyweights for words with a positive and negative connotation. Where a positive word like "amazing" 
		will have a positive weight, and a negative word like "work", in my opinion, has a negative connotation and would have a negative weight. 
		Words with neutral weight need not be included. Json analyzer then accepts the list of generic objects, runs the rules on each generic object 
		in the deserialized list (again mimicing the apis unstructured data), and stores the objects with their scores in a dictionary. I then use the
		getTop(n) method to get the top n reviews - in this case n is 3 per the requirement - with the highest positivity score and output them to the 
		console. 

		-- How to Test:
		I used NUnit to test my classes, and ran my test using visiual studio, and the test explorer. I do not believe there is another way to run 
		these tests.
		
		In visiual studio click on "view" > "test explorer"
		Click on the green double play button that says "run all tests in view"
		If you would like to examine my test cases you may see them in the second project called "CodingChallengeUnitTests"
		Each test class, corresponds to either WebScraper or JsonAnalyzer, and each method is a different equivalence class. There are multile tests 
		for the methods included in the corresponding class.

		The test classes achieve 100% Line and Condition coverage. Luckily the cyclomatic complexity of this project
		is not very high.

		-- How to Run: 
		Click ">CodingChallenge" in the visiual studio debugger to run it in debug, or you can run the executable by going executing the file at
		"CodingChallenge\bin\Debug\netcoreapp3.1\CodingChallenge.exe" This executable should work across all operating systems.


		-- Resources:
		URL of page to scrape:
		https://www.dealerrater.com/dealer/McKaig-Chevrolet-Buick-A-Dealer-For-The-People-dealer-reviews-23685

		DealerRater API conatact and Documentation:
		jamie@dealerrater.com 781 697 3579
		http://api.dealerrater.com/docs/
		https://partner.dealerrater.com/Documentation/Dealers/DealerReviewsCollection.aspx


		Example review element from the HTTP Request:
		<p class="font-16 review-content margin-bottom-none line-height-25">
		Awesome place! We went in afraid that we may not be able to get exactly what we wanted, however they were
		able to work with us and got us the exact vehicle we wanted within our budget. The people were awesome! 
		We didn’t feel rushed and they even stayed well after hours to take care of us! </p>

		The JSON representation for the collection of reviews is in the following format.
		{
			"reviews": [
				{
					"comments": "string"
				}
			]
		}

