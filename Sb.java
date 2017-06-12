package com.mycompany.mygame;

import java.util.Random;

public class Sb 
{
	private Random random = new Random();
	
	private static int cryptoKey = 215;	
	private int secureValue;    

    public Sb(boolean value)
    {
    	secureValue = encryptValue(value);
    	randomizeCryptoKey();
    }
    
    public void setValue(boolean value)
    {
    	secureValue = encryptValue(value);
    	randomizeCryptoKey();
    }

    public boolean getValue()
    {
        return decryptValue(secureValue);
    }
    
    private int encryptValue(boolean value)
    {
		int encryptedValue = value ? 213 : 181;
		encryptedValue ^= cryptoKey;
        
		return encryptedValue;
    }
    
    private int encryptValue(boolean value, int key)
    {
		int encryptedValue = value ? 213 : 181;
		encryptedValue ^= key;
		return encryptedValue;
    }
    
    private boolean decryptValue(int value)
    {
		value ^= cryptoKey;
		return value != 181;
    }

    @Override
    public String toString()
    {
    	return Boolean.toString(decryptValue(secureValue)); 
    }
    
    public Boolean toBoolean()
    {
    	return decryptValue(secureValue);
    }
	
	public void invertValue()
    {
		randomizeCryptoKey();
		setValue(!decryptValue(secureValue));
    }
	
	public void randomizeCryptoKey()
    {
		boolean decrypted =  decryptValue(secureValue);
		cryptoKey = 1 + random.nextInt(150);
		secureValue = encryptValue(decrypted, cryptoKey);
    }
}
