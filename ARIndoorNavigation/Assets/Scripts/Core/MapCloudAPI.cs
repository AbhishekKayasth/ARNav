using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class MapCloudAPI : MonoBehaviour
{
    const string BundleFolder = ""; // link to bundles here

    public void GetBundleObject(string assetName, UnityAction<GameObject> callback, Transform bundleParent)
	{
		StartCoroutine(GetDisplayBundleRoutine(assetName, callback, bundleParent));
	}

	IEnumerator GetDisplayBundleRoutine(string assetName, UnityAction<GameObject> callbacks, Transform bundleParent)
	{
		string bundleURL = BundleFolder + assetName + "-";

		// Append platform to asset bundle name
#if UNITY_ANDROID
		bundleURL += "Android";
#else
		bundleURL += "IOS";
#endif
		Debug.Log("Requesting bundle at " + bundleURL);

		// Request asset bundle
		UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL);
		yield return www.SendWebRequest();

		if(www.isNetworkError)
		{
			Debug.Log("Network Error");
		}
		else
		{
			AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
			if(bundle != null)
			{
				string rootAssetPath = bundle.GetAllAssetNames()[0];
				GameObject arObject = Instantiate(bundle.LoadAsset(rootAssetPath) as GameObject, bundleParent);
				bundle.Unload(false);
				callbacks(arObject);
			}
			else
			{
				Debug.Log("Not a valid asset bundle");
			}
		}
	}
}
