using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public class GamePlayBillingManager : MonoBehaviour
{
    private static ObscuredString BILLING_KEY_1 = "MIIBKKmkjfmncgHGl;b.kjH,njnf;lfmm";
    private static ObscuredString BILLING_KEY_2 = "IIBCjkfjHdlfHbvkHgelkfkmbkgHgdfkmg";
    private static ObscuredString BILLING_KEY_3 = "7fjkn34nj98dflbhjdflkfd843lknmgjnfnk";
    private static ObscuredString BILLING_KEY_4 = "kfdsj38ujdfj2093jjhjhgf893ujkfjdsklJf";
    private static ObscuredString BILLING_KEY_5 = "mkjju348()nfjkfhnjfhjn8hjkdsjlkjfljj894";
    private static ObscuredString BILLING_KEY_6 = "kJHnufndK934kHUghre93476Jlkjfuhjkfg";
    private static ObscuredString BILLING_KEY_7 = "JKJhfurek89KJHyhugfrelk*(kjhfl;sdfjUJh";
    private static ObscuredString BILLING_KEY_8 = "Jhk4jkhjkfmkdnmfHBjknflkdsHJnfdjfgkjjgn";
    private static ObscuredString BILLING_KEY_9 = "jmkfjuiHgdf348ugnkHG203486HJHGnfjjf";
    private static ObscuredString BILLING_KEY_10 = "jfhfHdkjghGGnmjfhf98HgffdkHnmvjfhG";
    private static ObscuredString BILLING_KEY_11 = "hfNgbhjk78429764567KhfKfgnKdfhDjfjfh";
    private static ObscuredString BILLING_KEY_12 = "jfhjhgLJhnfnmfgjYfkmgkfHnvfdkjGYdfm";

    private static ObscuredString pluginPackageName = "com.mycompany.mygame";
    private static AndroidJavaClass unityPlayer;
    private static AndroidJavaObject activity;
    private static AndroidJavaClass pluginVerifyClass;

    private static ObscuredString stringSignatureVerificationState = "false";

    void CallSignatureVerificationClass()
    {
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        pluginVerifyClass = new AndroidJavaClass(pluginPackageName + ".Sv");
    }
    public static void VerifySignatureValue()
    {
        pluginVerifyClass.CallStatic("Vs");
		stringSignatureVerificationState = pluginVerifyClass.CallStatic<string>("svs", new object[12] { BILLING_KEY_1, BILLING_KEY_2, BILLING_KEY_3, 
			BILLING_KEY_4, BILLING_KEY_5, BILLING_KEY_6, BILLING_KEY_7, BILLING_KEY_8, BILLING_KEY_9, BILLING_KEY_10, BILLING_KEY_11, BILLING_KEY_12 });
		//_textSignatureVerificatioResult.text = "Verify = " + stringSignatureVerificationState;
    }
    public static void purchase(string SKU)
    {
        VerifySignatureValue();
        if (stringSignatureVerificationState == "true")
        {
            AndroidInAppPurchaseManager.Client.Purchase(SKU);
        }
    }
}
