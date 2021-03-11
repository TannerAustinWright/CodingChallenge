		
		Podium Coding Challenge
		Tanner Wright

		Coding Challenge: “A Dealer For the People”
		The KGB has noticed a resurgence of overly excited reviews for a McKaig Chevrolet Buick, a dealership they have planted in the United States. 
		In order to avoid attracting unwanted attention, you’ve been enlisted to scrape reviews for this dealership from DealerRater.com and uncover 
		the top three worst offenders of these overly positive endorsements.

		Your mission, should you choose to accept it, is to write a tool that:

		1 scrapes the first five pages of reviews
		2 identifies the top three most “overly positive” endorsements (using criteria of your choosing, documented in the README)
		3 outputs these three reviews to the console, in order of severity
		Please include documentation on how to run your application along with how to run the test suite. 

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

