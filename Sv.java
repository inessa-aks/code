package com.powerenergetics.GooglePlayGamesCloud;

import java.security.KeyFactory;
import java.security.PublicKey;
import java.security.Signature;
import java.security.spec.X509EncodedKeySpec;

import android.util.Base64;

public class Sv 
{	
	final static String LOG_TAG = "myLogs";
	
	static Sb signatureVerificationState = new Sb(false);
	static String svs = "false";
	
	static String signatureValue = "A4J3fP2LLhK0H3KJ3GT08Jg8LkaYS3GN";
	static String signedData = "LO2Jh5AN9Sd2FvX1FuP9Dc7Sx3OGDd8B";
    
	public static void Vs(String base1, String base2, String base3, String base4, String base5, String base6, 
			String base7, String base8, String base9, String base10, String base11, String base12)
	{
		signatureVerificationState = new Sb(false);
		svs = signatureVerificationState.toString();
		try
		{
	        X509EncodedKeySpec spec = new X509EncodedKeySpec(Base64.decode(
	        				(base1 + base2 + base3 + base4 + base5 + base6 + 
	        				 base7 + base8 + base9 + base10 + base11 + base12)
	        				.getBytes(), Base64.DEFAULT));
			KeyFactory keyFactory = KeyFactory.getInstance("RSA");
			PublicKey publicKey = keyFactory.generatePublic(spec); 
			Signature signature = Signature.getInstance("SHA1withRSA");
			signature.initVerify(publicKey);
			signature.update(signedData.getBytes());
			Sb result = new Sb(signature.verify(signatureValue.getBytes("UTF-8"))); 
			signatureVerificationState = result;
			svs = result.toString();
		}
		catch (Exception e)
		{
		}
	}
}
