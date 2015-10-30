using UnityEngine;
using OnePF;
using System.Collections.Generic;

public class InAppPurchaseOpenIAB : MonoBehaviour
{
	const string SKU = "subscribe_period";
	string extraMessage = "Message";
	
	string _label = "";
	bool _isInitialized = false;
	Inventory _inventory = null;
	
	const float X_OFFSET = 10.0f;
	const float Y_OFFSET = 10.0f;
	const int SMALL_SCREEN_SIZE = 800;
	const int LARGE_FONT_SIZE = 34;
	const int SMALL_FONT_SIZE = 24;
	const int LARGE_WIDTH = 380;
	const int SMALL_WIDTH = 160;
	const int LARGE_HEIGHT = 100;
	const int SMALL_HEIGHT = 40;
	
	int _column = 0;
	int _row = 0;
	
	private void Start()
	{
		// Map skus for different stores       
		OpenIAB.mapSku(SKU, OpenIAB_Android.STORE_GOOGLE, "subscribe_period");
	}
	
	private void OnGUI()
	{
		_column = 0;
		_row = 0;
		
		GUIStyle guiBoxStyle = new GUIStyle("box");
		guiBoxStyle.fontSize = 30;
		
		if (extraMessage != null) {
			guiBoxStyle.alignment = TextAnchor.UpperLeft;
			guiBoxStyle.wordWrap = true;
			GUI.Box (new Rect (10, 390, Screen.width - 20, Screen.height - 400), extraMessage, guiBoxStyle);
		}
		
		GUI.skin.button.fontSize = (Screen.width >= SMALL_SCREEN_SIZE || Screen.height >= SMALL_SCREEN_SIZE) ? LARGE_FONT_SIZE : SMALL_FONT_SIZE;
		
		if (Button("Initialize OpenIAB"))
		{
			// Application public key
			var googlePublicKey = "";
			
			var options = new Options();
			options.checkInventoryTimeoutMs = Options.INVENTORY_CHECK_TIMEOUT_MS * 2;
			options.discoveryTimeoutMs = Options.DISCOVER_TIMEOUT_MS * 2;
			options.checkInventory = false;
			options.verifyMode = OptionsVerifyMode.VERIFY_SKIP;
			options.storeKeys = new Dictionary<string, string> { {OpenIAB_Android.STORE_GOOGLE, googlePublicKey} };
			options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;
			
			// Transmit options and start the service
			OpenIAB.init(options);
		}
		
		if (!_isInitialized)
			return;
		
		if (Button("Subscribe Product"))
		{
			OpenIAB.purchaseSubscription(SKU);
		}
	}
	
	private void OnEnable()
	{
		// Listen to all events for illustration purposes
		OpenIABEventManager.billingSupportedEvent += billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent += purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
	}
	
	private void OnDisable()
	{
		// Remove all event handlers
		OpenIABEventManager.billingSupportedEvent -= billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}
	
	private bool Button(string text)
	{
		float width = Screen.width / 2.0f - X_OFFSET * 2;
		float height = (Screen.width >= SMALL_SCREEN_SIZE || Screen.height >= SMALL_SCREEN_SIZE) ? LARGE_HEIGHT : SMALL_HEIGHT;
		
		bool click = GUI.Button(new Rect(
			X_OFFSET + _column * X_OFFSET * 2 + _column * width, 
			Y_OFFSET + _row * Y_OFFSET + _row * height, 
			width, height),
		                        text);
		
		++_column;
		if (_column > 1)
		{
			_column = 0;
			++_row;
		}
		
		return click;
	}
	
	
	private void billingSupportedEvent()
	{
		_isInitialized = true;
		Debug.Log("billingSupportedEvent");
		extraMessage = "billingSupportedEvent: True";
	}
	private void billingNotSupportedEvent(string error)
	{
		Debug.Log("billingNotSupportedEvent: " + error);
		extraMessage = "billingNotSupportedEvent: " + error;
	}
	private void queryInventorySucceededEvent(Inventory inventory)
	{
		Debug.Log("queryInventorySucceededEvent: " + inventory);
		if (inventory != null)
		{
			_label = inventory.ToString();
			_inventory = inventory;
			extraMessage = "queryInventorySucceededEvent: " + _label;
		}
	}
	private void queryInventoryFailedEvent(string error)
	{
		Debug.Log("queryInventoryFailedEvent: " + error);
		_label = error;
		extraMessage = "queryInventoryFailedEvent: " + error;
	}
	private void purchaseSucceededEvent(Purchase purchase)
	{
		Debug.Log("purchaseSucceededEvent: " + purchase);
		_label = "PURCHASED:" + purchase.ToString();
		extraMessage = "purchaseSucceededEvent: " + _label;
	}
	private void purchaseFailedEvent(int errorCode, string errorMessage)
	{
		Debug.Log("purchaseFailedEvent: " + errorMessage);
		_label = "Purchase Failed: " + errorMessage;
		extraMessage = "purchaseFailedEvent: " + _label;
	}
	private void consumePurchaseSucceededEvent(Purchase purchase)
	{
		Debug.Log("consumePurchaseSucceededEvent: " + purchase);
		_label = "CONSUMED: " + purchase.ToString();
		extraMessage = "consumePurchaseSucceededEvent: " + _label;
	}
	private void consumePurchaseFailedEvent(string error)
	{
		Debug.Log("consumePurchaseFailedEvent: " + error);
		_label = "Consume Failed: " + error;
		extraMessage = "consumePurchaseFailedEvent: " + _label;
	}
}
