using System.Collections.Generic;
using UnityEngine;

public class GlobalMng
{
    private static Transform globalTr;

    private static Dictionary<string,MonoBehaviour> globalSingletonDict = new Dictionary<string, MonoBehaviour>();

    public static T GlobalSingleton<T>() where T : MonoBehaviour
    {
        string type = typeof (T).Name;

        if (globalTr == null)
        {
            GameObject go = new GameObject("GlobalMng");
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            globalTr = go.transform;
            GameObject.DontDestroyOnLoad(go);
        }

        T singleton = null;
        if (!globalSingletonDict.ContainsKey(type))
        {
            GameObject singletonGo = new GameObject(type);
            singletonGo.transform.SetParent(globalTr);
            singletonGo.transform.localPosition = Vector3.zero;
            singletonGo.transform.localScale = Vector3.one;

            singleton = singletonGo.AddComponent<T>();

            globalSingletonDict.Add(type,singleton);
        }
        else
        {
            singleton = (T)globalSingletonDict[type];
        }

        return singleton;
    }
}
