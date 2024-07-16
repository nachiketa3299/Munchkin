using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using UnityEngine;
using UnityEngine.Serialization;

using Vector3 = System.Numerics.Vector3;

public class DataSaver : MonoBehaviour
{
	private string savePath = Application.dataPath;

	[FormerlySerializedAs("myData")]
	[SerializeField]
	private GameData myGameData;

	[ContextMenu("SaveTest")]
	public void SaveData()
	{
		savePath = Application.dataPath + "\\SaveData.json";
		Debug.Log(savePath);

		string json = JsonUtility.ToJson(myGameData, true);
		json = Encrypt(json, "sss");
		File.WriteAllText(savePath, json);
	}

	[ContextMenu("LoadTest")]
	public void LoadData()
	{
		savePath = Application.dataPath + "\\SaveData.json";
		string data = File.ReadAllText(savePath);
		data = Decrypt(data, "sss");
		GameData loadGameData = JsonUtility.FromJson<GameData>(data);
		myGameData = loadGameData;
	}

	#region AES

	public static string Decrypt(string textToDecrypt, string key)
	{
		RijndaelManaged rijndaelCipher = new RijndaelManaged();

		rijndaelCipher.Mode = CipherMode.CBC;

		rijndaelCipher.Padding = PaddingMode.PKCS7;


		rijndaelCipher.KeySize = 128;

		rijndaelCipher.BlockSize = 128;

		byte[] encryptedData = Convert.FromBase64String(textToDecrypt);

		byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

		byte[] keyBytes = new byte[16];

		int len = pwdBytes.Length;

		if (len > keyBytes.Length)

		{
			len = keyBytes.Length;
		}

		Array.Copy(pwdBytes, keyBytes, len);

		rijndaelCipher.Key = keyBytes;

		rijndaelCipher.IV = keyBytes;

		byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

		return Encoding.UTF8.GetString(plainText);
	}


	public static string Encrypt(string textToEncrypt, string key)

	{
		RijndaelManaged rijndaelCipher = new RijndaelManaged();

		rijndaelCipher.Mode = CipherMode.CBC;

		rijndaelCipher.Padding = PaddingMode.PKCS7;


		rijndaelCipher.KeySize = 128;

		rijndaelCipher.BlockSize = 128;

		byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

		byte[] keyBytes = new byte[16];

		int len = pwdBytes.Length;

		if (len > keyBytes.Length)

		{
			len = keyBytes.Length;
		}

		Array.Copy(pwdBytes, keyBytes, len);

		rijndaelCipher.Key = keyBytes;

		rijndaelCipher.IV = keyBytes;

		ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

		byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);

		return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
	}

	#endregion
}

[System.Serializable]
public class GameData
{
	//public DateTime SaveTime;
	public List<string> ActiveSceneName;
	public PlayerData playerData;
}

[System.Serializable]
public class PlayerData
{
	public Vector3 position;
}