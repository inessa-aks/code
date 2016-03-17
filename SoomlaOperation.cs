using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using Soomla;
using Soomla.Profile;
using Grow.Highway;
using Grow.Insights;

public class SoomlaOperation : MonoBehaviour {

	public GameObject facebookTextGameObject;
	public GameObject googleTextGameObject;
	public GameObject twitterTextGameObject;

	[HideInInspector]
	public static Provider targetProvider;
	public static Reward targetReward = new BadgeReward("target_reward", "Reward");

	private Texture2D fileTexture = null;

	//
	// Various event handling methods
	//
	public void onGrowInsightsInitialized () {
		Debug.Log("Grow insights has been initialized.");
	}
	public void onInsightsRefreshFinished (){
		if (GrowInsights.UserInsights.PayInsights.PayRankByGenre[Genre.Educational] > 3) {
			// ... Do stuff according to your business plan ...
		}
	}

	IEnumerator LoadImageFromUrl()
	{
		using (WWW www = new WWW(@"http://assets.soom.la/soombots/spockbot.png"))
		{
			yield return www;
			fileTexture = www.texture;
			if (!string.IsNullOrEmpty (www.error)) 
			{
				Debug.Log(www.error);
			}
			
		}
	}

	// Use this for initialization
	void Start () {
		
		// Setup all event handlers - Make sure to set the event handlers before you initialize

		//HighwayEvents.OnGrowInsightsInitialized += onGrowInsightsInitialized;
		//HighwayEvents.OnInsightsRefreshFinished += onInsightsRefreshFinished;
		
		// Make sure to make this call in your earliest loading scene,
		// and before initializing any other SOOMLA/GROW components
		// i.e. before SoomlaStore.Initialize(...)

		//GrowHighway.Initialize();
		
		// Make sure to make this call AFTER initializing HIGHWAY

		//GrowInsights.Initialize();
		
		// Initialize the other SOOMLA modules you're using here

		// Catching fired events
		ProfileEvents.OnSoomlaProfileInitialized += () => {
			Soomla.SoomlaUtils.LogDebug("SoomlaOperation", "SoomlaProfile Initialized !");
		};

		ProfileEvents.OnLoginStarted += (Provider targetProvider, bool autoLogin, string payload) => {
			Soomla.SoomlaUtils.LogDebug("SoomlaOperation", "SoomlaProfile - OnLoginStarted !");
		};

		ProfileEvents.OnLoginFailed += (Provider targetProvider, string message, bool autoLogin, string payload) => {
			Soomla.SoomlaUtils.LogDebug("SoomlaOperation", "SoomlaProfile - OnLoginFailed !");

			if (targetProvider == Provider.FACEBOOK)
			{
				Text text = facebookTextGameObject.GetComponent<Text>();
				text.text = message;
			}
			if (targetProvider == Provider.GOOGLE)
			{
				Text text = googleTextGameObject.GetComponent<Text>();
				text.text = message;
			}
			if (targetProvider == Provider.TWITTER)
			{
				Text text = twitterTextGameObject.GetComponent<Text>();
				text.text = message;
			}

		};

		ProfileEvents.OnLoginFinished += (UserProfile UserProfile, bool autoLogin, string payload) => {
			Soomla.SoomlaUtils.LogDebug("SoomlaOperation", "Login finished for: " + UserProfile.toJSONObject().print());

			targetProvider = UserProfile.Provider;

			if (targetProvider == Provider.FACEBOOK)
			{
				Text text = facebookTextGameObject.GetComponent<Text>();
				text.text = "Facebook login successfull";
			}

			if (targetProvider == Provider.GOOGLE)
			{
				Text text = googleTextGameObject.GetComponent<Text>();
				text.text = "Google login successfull";
			}

			if (targetProvider == Provider.TWITTER)
			{
				Text text = twitterTextGameObject.GetComponent<Text>();
				text.text = "Twitter login successfull";
			}

			if (payload == "FacebookUpdateStory")
			{
				targetProvider = Provider.FACEBOOK;
				SoomlaProfileUpdateStoryDialog(targetProvider);
			}

			if (payload == "GooglePlusUpdateStory")
			{
				targetProvider = Provider.GOOGLE;
				SoomlaProfileUpdateStoryDialog(targetProvider);
			}

			if (payload == "TwitterUpdateStory")
			{
				targetProvider = Provider.TWITTER;
				SoomlaProfileUpdateStoryDialog(targetProvider);
			}
		}; // ProfileEvents.OnLoginFinished

		SoomlaProfile.Initialize();
				
		#if UNITY_IPHONE
		Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.Gray);
		#elif UNITY_ANDROID
		Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
		#endif
	}

	public void OnGoogleLoginCheck()
	{
		targetProvider = Provider.GOOGLE;
		Text text = googleTextGameObject.GetComponent<Text>();
		
		if (SoomlaProfile.IsLoggedIn(targetProvider)) 
			text.text = "Google+ is logined";
		else 
			text.text = "Google+ is not logined";
	}

	public void OnGoogleUpdate()
	{
		targetProvider = Provider.GOOGLE;
		StartCoroutine(LoadImageFromUrl());
		SoomlaProfileUpdateStoryDialog(targetProvider);
	}
	
	public void OnGoogleLogin()
	{
		targetProvider = Provider.GOOGLE;
		SoomlaProfile.Login(targetProvider, "GooglePlusUpdateStory", targetReward);
	}
	
	public void OnGoogleLogOut()
	{
		targetProvider = Provider.GOOGLE;
		SoomlaProfile.Logout(targetProvider);
	}
	
	public void OnTwitterLoginCheck()
	{
		targetProvider = Provider.TWITTER;
		Text text = twitterTextGameObject.GetComponent<Text>();
		
		if (SoomlaProfile.IsLoggedIn(targetProvider)) 
			text.text = "Twitter is logined";
		else 
			text.text = "Twitter is not logined";
	}
		
	public void OnTwitterUpdate()
	{
		targetProvider = Provider.TWITTER;
		SoomlaProfileUpdateStoryDialog(targetProvider);
	}

	public void OnTwitterLogin()
	{
		targetProvider = Provider.TWITTER;
		SoomlaProfile.Login(targetProvider, "TwitterUpdateStory", targetReward);
	}
	
	public void OnTwitterLogOut()
	{
		targetProvider = Provider.TWITTER;
		SoomlaProfile.Logout(targetProvider);
	}

	public void OnFacebookUpdate()
	{
		targetProvider = Provider.FACEBOOK;
		StartCoroutine(LoadImageFromUrl());
		SoomlaProfileUpdateStoryDialog(targetProvider);	
	}
	
	public void OnFacebookLoginCheck()
	{
		targetProvider = Provider.FACEBOOK;
		Text text = facebookTextGameObject.GetComponent<Text>();
		
		if (SoomlaProfile.IsLoggedIn(targetProvider)) 
			text.text = "Facebook is logined";
		else 
			text.text = "Facebook is not logined";
	}
	
	public void OnFacebookLogin()
	{
		targetProvider = Provider.FACEBOOK;
		SoomlaProfile.Login(targetProvider, "FacebookUpdateStory", targetReward);
	}
	
	public void OnFacebookLogOut()
	{
		targetProvider = Provider.FACEBOOK;
		SoomlaProfile.Logout(targetProvider);
	}

	void SoomlaProfileUpdateStoryDialog(Provider TargetProvider)
	{
		SoomlaProfile.UploadImage(
				TargetProvider,     // Provider
				"Message",          // Message to post with image (string)
				"spockbot.png",    // File name (string)
				fileTexture,         // File Texture (Texture2D)
				"",                     // Payload (string payload = "")
				null                   // Reward (Reward reward = null)
				);	

/*
		SoomlaProfile.UpdateStory(
			TargetProvider,                                                   // Provider
			"This is the story.",                                             // Text of the story to post
			"The story of SOOMBOT (Profile Test App)",           // Name
			"SOOMBOT Story",                                              // Caption
			"Hey! It's SOOMBOT Story",                                // Description
			"http://about.soom.la/soombots",                       // Link to post
			"http://assets.soom.la/soombots/spockbot.png", // Image URL
			"",                                                                   // Payload
			null                                                                 // Reward for posting a story
			);
/*
		SoomlaProfile.UpdateStoryDialog(
			TargetProvider,                                                   // Provider
			"The story of SOOMBOT (Profile Test App)",          // Name
			"SOOMBOT Story",                                             // Caption
			"Hey! It's SOOMBOT Story",                               // Description
			"http://about.soom.la/soombots",                      // Link to post
			"http://assets.soom.la/soombots/spockbot.png", // Image URL
			"",                                                                   // Payload
			null                                                                 // Reward
			);
*/	
	}
}
