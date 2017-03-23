using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static List<UIBase> uiStack = new List<UIBase>();

    private static UILoader loader = new UILoader();

    private static Dictionary<string,List<UICommon.UIMsgCallback>> uiMsgDict = new Dictionary<string, List<UICommon.UIMsgCallback>>(); 

    private static Dictionary<string,UIBase> uiCache = new Dictionary<string, UIBase>(); 

    private static UIManager instance;

    public static UIBase CurrentUIPanel;

    void Awake()
    {
        instance = this;
    }

    public static void OpenPanel(string uiName,bool closeBottom = false)
    {
        UIBase panel = uiStack.Find((p) => p.UIName.Equals(uiName));

        if (panel == null)
        {

            if (uiCache.ContainsKey(uiName))
            {
                panel = uiCache[uiName];
            }
            else
            {
                GameObject uiPrefab = loader.GetUIByName(uiName);

                GameObject ui = Instantiate(uiPrefab);
                panel = ui.GetComponent<UIBase>();
                panel.UIName = uiName;
                panel.OnInit();
            }

            CurrentUIPanel = panel;

            panel.gameObject.SetActive(true);
            panel.transform.SetParent(instance.transform);
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.one;

            panel.OnRefresh();

            if (uiStack.Count > 0)
            {
                UIBase lastPanel = uiStack[uiStack.Count - 1];
                lastPanel.gameObject.SetActive(false);
            }

            uiStack.Add(panel);
        }
        else
        {
            Debug.LogError(uiName + "已经打开");
        }

    }

    public static void ClosePanel(string name)
    {
        UIBase uiPanel = uiStack.Find((p) => p.UIName.Equals(name));

        if (uiPanel != null)
        {
            uiPanel.gameObject.SetActive(false);

            uiStack.Remove(uiPanel);

            uiCache.Add(uiPanel.UIName, uiPanel);

            if (uiStack.Count > 0)
            {
                uiStack[uiStack.Count - 1].gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("没有找到要" + name);
        }
    }

    public static void CloseTop()
    {
        if (uiStack.Count > 0)
        {
            UIBase uiPanel = uiStack[uiStack.Count - 1];

            uiPanel.OnBeginClose();
            uiPanel.gameObject.SetActive(false);
            uiPanel.OnEndClose();

            uiStack.Remove(uiPanel);
            uiCache.Add(uiPanel.UIName, uiPanel);
        }
    }

    public static void DispatchMsg(string msgType,UICommon.UIMsg msg)
    {

    }

    public static void AddListener(string msgType,UICommon.UIMsgCallback callback)
    {
        if (uiMsgDict.ContainsKey(msgType))
        {
            if (callback != null)
            {
                if (!uiMsgDict[msgType].Contains(callback))
                {
                    uiMsgDict[msgType].Add(callback);
                }
                else
                {
                    Debug.LogError("消息" + msgType + "已经注册过了");
                }
            }
        }
        else
        {
            uiMsgDict.Add(msgType,new List<UICommon.UIMsgCallback>());
            uiMsgDict[msgType].Add(callback);
        }
    }
}
