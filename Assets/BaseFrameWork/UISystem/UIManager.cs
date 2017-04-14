using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI管理提，负责UI的关闭，打开相关调度
/// </summary>
public class UIManager : MonoBehaviour
{

    public static UIBase CurrentUIPanel;

    public static Camera UICamera;

    public static Canvas UICanvas;

    private static UIManager instance;

    private List<UIBase> uiStack = new List<UIBase>();

    private UILoader loader = new UILoader();

    private Dictionary<string,List<UIMsgCallback>> uiMsgDict = new Dictionary<string, List<UIMsgCallback>>(); 

    private Dictionary<string,UIBase> uiCache = new Dictionary<string, UIBase>(); 

	private CanvasScaler canvasScaler;

    private RectTransform panelRoot;

    void Awake()
    {
        instance = this;
		canvasScaler = GetComponent<CanvasScaler> ();
        UICanvas = GetComponent<Canvas>();
        GameObject camera = GameObject.Find("UICamera");
        GameObject eventSystem = GameObject.Find("EventSystem");

        UICamera = camera.GetComponent<Camera>();

        GameObject panelRootGo = new GameObject("UIPanels");
        panelRoot = panelRootGo.AddComponent<RectTransform>();
        panelRoot.SetParent(transform);
        panelRoot.localPosition = Vector3.zero;
        panelRoot.localScale = Vector3.one;
        panelRoot.anchorMax = new Vector2(1, 1);
        panelRoot.anchorMin = new Vector2(0, 0);
        panelRoot.offsetMax = new Vector2(0, 0);
        panelRoot.offsetMin = new Vector2(0, 0);

        DontDestroyOnLoad(camera);
        DontDestroyOnLoad(eventSystem);
        DontDestroyOnLoad(gameObject);
    }

    public static void OpenPanel(string uiName,bool closeBottom = false,params object[] args)
    {
        if (instance == null)
        {
            return;
        }
        instance._OpenPanel(uiName,closeBottom,args);
    }

    public static void ClosePanel(string name)
    {
        instance._ClosePanel(name);
    }

    public static void CloseTop()
    {
        instance._CloseTop();
    }

    public static void AddListener(string msgType, UIMsgCallback callback)
    {
        instance._AddListener(msgType, callback);
    }

    public static void DispatchMsg(string msgType,params object[] args)
    {
        if (instance == null)
        {
            return;
        }
        instance._DispatchMsg(msgType,args);
    }

    private void _OpenPanel(string uiName, bool closeBottom = false, params object[] args)
    {
        UIBase panel = uiStack.Find((p) => p.UIName.Equals(uiName));

        if (panel == null)
        {

            if (uiCache.ContainsKey(uiName))
            {
                panel = instance.uiCache[uiName];
                instance.uiCache.Remove(uiName);
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
            panel.Args = args;
            panel.gameObject.SetActive(true);
            panel.transform.SetParent(instance.panelRoot);
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.one;

            RectTransform rtf = panel.GetComponent<RectTransform>();
            rtf.anchorMax = new Vector2(1, 1);
            rtf.anchorMin = new Vector2(0, 0);
            rtf.offsetMax = new Vector2(0, 0);
            rtf.offsetMin = new Vector2(0, 0);

            panel.OnRefresh();

            if (uiStack.Count > 0 && closeBottom)
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

    private void _ClosePanel(string name)
    {
        UIBase uiPanel = uiStack.Find((p) => p.UIName.Equals(name));

        if (uiPanel != null)
        {
            uiPanel.gameObject.SetActive(false);
            uiStack.Remove(uiPanel);
            uiCache.Add(uiPanel.UIName, uiPanel);
        }
        else
        {
            Debug.LogError("没有找到要关闭的" + name);
        }
    }

    private void _CloseTop()
    {
        if (uiStack.Count > 0)
        {
            UIBase uiPanel = uiStack[uiStack.Count - 1];

            uiPanel.OnBeginClose();
            uiPanel.gameObject.SetActive(false);
            uiPanel.OnEndClose();

            uiStack.Remove(uiPanel);
            uiCache.Add(uiPanel.UIName, uiPanel);

            if (uiStack.Count > 0)
            {
                uiStack[uiStack.Count - 1].gameObject.SetActive(true);
                uiStack[uiStack.Count - 1].OnRefresh();
            }
        }
    }



    private void _DispatchMsg(string msgType, params object[] args)
    {
        UIMsg m = new UIMsg();

        m.MsgType = msgType;
        m.args = args;

        if (uiMsgDict.ContainsKey(msgType))
        {
            List<UIMsgCallback> listenerList = uiMsgDict[msgType];
            for (int i = 0; i < listenerList.Count; i++)
            {
                UIMsgCallback listener = listenerList[i];
                if (listener.Method.IsStatic)
                {
                    listener.Invoke(m);
                }
                else if (listener.Target != null)
                {
                    listener.Invoke(m);
                }
            }
        }
    }


    private void _AddListener(string msgType, UIMsgCallback callback)
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
            uiMsgDict.Add(msgType, new List<UIMsgCallback>());
            uiMsgDict[msgType].Add(callback);
        }
    }
}
