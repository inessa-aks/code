using UnityEngine;
using System.Collections;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;


public class AutomaticAdvertizement : MonoBehaviour, IBannerAdListener {

	#if UNITY_EDITOR
	string appKey = "a23f1c3c5f3a996c6f92ffdb6cb7f52ba14b26e45aab3188";
	#elif UNITY_ANDROID
	string appKey = "dd1af00d3ec8dbf21772d1c8e6d949ccb7a96796fd89753d";
	#elif UNITY_IPHONE
	string appKey = "dee74c5129f53fc629a44a690a02296694e3eef99f2d3a5f";
	#else
	string appKey = "unexpected_platform";
	#endif

	// Use this for initialization
	void Start () {
		Appodeal.initialize (appKey);
		//Appodeal.setTesting(true);
		Appodeal.setBannerCallbacks (this);
		Appodeal.show(Appodeal.BANNER_BOTTOM);
	}

	#region Banner callback handlers
	
	public void onBannerLoaded() {
		print("Banner loaded");
		//Appodeal.show(Appodeal.BANNER);
	}
	public void onBannerFailedToLoad() { print("Banner failed"); }
	public void onBannerShown() { print("Banner opened"); }
	public void onBannerClicked() { print("banner clicked"); }
	
	#endregion
	
	// Update is called once per frame
	void Update () {
	
	}
}
