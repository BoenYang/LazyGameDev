
using System.Collections.Generic;
using UnityEngine;

public class UILoader {

    private Dictionary<string,GameObject> cachedUI = new Dictionary<string, GameObject>();

    private static readonly string UIPrefabPath = "UI/";

    public GameObject GetUIByName(string name)
    {
        if (cachedUI.ContainsKey(name))
        {
            return cachedUI[name];
        }
        GameObject go = LoadUI(name);

        if (go != null)
        {
            cachedUI.Add(name,go);
        }
        return go;
    }

    protected virtual GameObject LoadUI(string name)
    {
        return Resources.Load<GameObject>(UIPrefabPath + name);
    }

}
