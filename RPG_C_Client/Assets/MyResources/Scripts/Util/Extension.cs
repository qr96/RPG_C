using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Util.GetOrAddComponent<T>(go);
	}

	public static bool IsValid(this GameObject go)
	{
		return go != null && go.activeSelf;
	}

    public static GameObject Find(this GameObject go, string path)
    {
		var names = path.Split("/");
		var parent = go.transform;

		foreach (var name in names)
            parent = parent.Find(name);

        return parent.gameObject;
    }

    public static T Find<T>(this GameObject go, string path) where T : UnityEngine.Component
	{
		var component = go.Find(path).GetComponent<T>();
		if (component == null) Debug.LogError("Try to find wrong component");
		return component;
	}
}
