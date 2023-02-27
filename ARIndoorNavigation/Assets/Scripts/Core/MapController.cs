using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
	public MapCloudAPI api;
    public void LocalMap(string name)
	{
		DestroyAllChildren();
		api.GetBundleObject(name, OnMapLoaded, transform);
	}

	void OnMapLoaded(GameObject map)
	{
		// Anything extra should be done here
		Debug.Log("Loaded: " + map.name);
	}

	private void DestroyAllChildren()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
}
